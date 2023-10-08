using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace ZLogger.Generator;

public partial class ZLoggerGenerator
{
	public record ParseResult(
		TypeDeclarationSyntax TargetTypeSyntax,
		INamedTypeSymbol TargetTypeSymbol,
		LogMethodDeclaration[] LogMethods);

	public record LogMethodDeclaration(
		ZLoggerMessageAttribute Attribute,
		IMethodSymbol TargetMethod,
		MethodDeclarationSyntax TargetSyntax,
		MessageSegment[] MessageSegments,
		MethodParameter[] MethodParameters);

	public class MethodParameter
	{
		public required IParameterSymbol Symbol { get; init; }
		public bool IsFirstLogger { get; init; }
		public bool IsFirstLogLevel { get; init; }
		public bool IsFirstException { get; init; }

		public bool IsParameter => !IsFirstLogger && !IsFirstLogLevel && !IsFirstException;

		// set from outside, if many segments was linked, use first-one.
		public MessageSegment LinkedMessageSegment { get; set; } = default!;

		public string ConvertJsonWriteMethod()
		{
			var type = Symbol.Type;
			switch (type.SpecialType)
			{
				case SpecialType.System_Enum:
					// TODO:Enum handling(Value or String, which should be default???)
					return "";
				case SpecialType.System_Boolean:
					return $"writer.WriteBoolean(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{Symbol.Name});";
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return $"writer.WriteNumber(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{Symbol.Name});";
				case SpecialType.System_String:
					return $"writer.WriteString(_jsonParameter_{LinkedMessageSegment.NameParameter}, this.{Symbol.Name});";
				default:
					return $"writer.WritePropertyName(_jsonParameter_{LinkedMessageSegment.NameParameter}); JsonSerializer.Serialize(writer, this.{Symbol.Name});";
			}
		}
	}

	public class Parser
	{
		SourceProductionContext context;
		ImmutableArray<GeneratorAttributeSyntaxContext> sources;

		INamedTypeSymbol loggerSymbol;
		INamedTypeSymbol logLevelSymbol;
		INamedTypeSymbol exceptionSymbol;

		public Parser(SourceProductionContext context, ImmutableArray<GeneratorAttributeSyntaxContext> sources)
		{
			this.context = context;
			this.sources = sources;

			var compilation = sources[0].SemanticModel.Compilation;
			this.loggerSymbol = GetTypeByMetadataName(compilation, "Microsoft.Extensions.Logging.ILogger");
			this.logLevelSymbol = GetTypeByMetadataName(compilation, "Microsoft.Extensions.Logging.LogLevel");
			this.exceptionSymbol = GetTypeByMetadataName(compilation, "System.Exception");
		}

		static INamedTypeSymbol GetTypeByMetadataName(Compilation compilation, string metadataName)
		{
			var symbol = compilation.GetTypeByMetadataName(metadataName);
			if (symbol == null)
			{
				throw new InvalidOperationException($"Type {metadataName} is not found in compilation.");
			}
			return symbol;
		}

		public ParseResult[] Parse()
		{
			var list = new List<ParseResult>();

			// grouping by type(TypeDeclarationSyntax)
			foreach (var item in sources.GroupBy(x => x.TargetNode.Parent))
			{
				if (item.Key == null) continue;
				var targetType = (TypeDeclarationSyntax)item.Key;

				var logMethods = new List<LogMethodDeclaration>();

				foreach (var source in item)
				{
					var method = (IMethodSymbol)source.TargetSymbol;
					var (attr, setLogLevel) = GetAttribute(source);
					var msg = attr.Message;

					if (!MessageParser.TryParseFormat(attr.Message, out var segments))
					{
						// TODO:Verify
						continue;
					}

					var parameters = GetMethodParameters(method, setLogLevel);

					// Set LinkedParameters
					foreach (var p in parameters.Where(x => x.IsParameter))
					{
						p.LinkedMessageSegment = segments
							.Where(x => x.Kind == MessageSegmentKind.NameParameter)
							.FirstOrDefault(x => x.NameParameter.Equals(p.Symbol.Name, StringComparison.OrdinalIgnoreCase));
					}

					if (!Verify())
					{
						continue;
					}

					var methodDecl = new LogMethodDeclaration(
						Attribute: attr,
						TargetMethod: (IMethodSymbol)source.TargetSymbol,
						TargetSyntax: (MethodDeclarationSyntax)source.TargetNode,
						MessageSegments: segments,
						MethodParameters: parameters);

					logMethods.Add(methodDecl);
				}

				var symbol = item.First().SemanticModel.GetDeclaredSymbol(targetType);
				var result = new ParseResult(targetType, symbol!, logMethods.ToArray());
				list.Add(result);
			}

			return list.ToArray();
		}

		static (ZLoggerMessageAttribute attr, bool setLogLevel) GetAttribute(GeneratorAttributeSyntaxContext source)
		{
			// TODO: Attribute verify.

			var attributeData = source.Attributes[0];

			int eventId = -1;
			string? eventName = null;
			LogLevel level = LogLevel.None;
			string message = "";
			bool skipEnabledCheck = false;

			// check logLevel is set for verify.
			var setLogLevel = false;
			var ctorItems = attributeData.ConstructorArguments;

			switch (ctorItems.Length)
			{
				case 2:
					// ZLoggerMessageAttribute(LogLevel level, string message)
					setLogLevel = true;
					level = ctorItems[0].IsNull ? LogLevel.None : (LogLevel)ctorItems[0].Value!;
					message = ctorItems[1].IsNull ? "" : (string)ctorItems[1].Value!;
					break;

				case 3:
					// ZLoggerMessageAttribute(int eventId, LogLevel level, string message)
					setLogLevel = true;
					eventId = ctorItems[0].IsNull ? -1 : (int)ctorItems[0].Value!;
					level = ctorItems[1].IsNull ? LogLevel.None : (LogLevel)ctorItems[1].Value!;
					message = ctorItems[2].IsNull ? "" : (string)ctorItems[2].Value!;
					break;
			}

			if (attributeData.NamedArguments.Any())
			{
				foreach (var namedArgument in attributeData.NamedArguments)
				{
					var typedConstant = namedArgument.Value;
					if (typedConstant.Kind == TypedConstantKind.Error)
					{
						break;
					}
					else
					{
						var value = namedArgument.Value;
						switch (namedArgument.Key)
						{
							case "EventId":
								eventId = (int)value.Value!;
								break;
							case "Level":
								setLogLevel = !value.IsNull;
								level = value.IsNull ? LogLevel.None : (LogLevel)value.Value!;
								break;
							case "SkipEnabledCheck":
								skipEnabledCheck = (bool)value.Value!;
								break;
							case "EventName":
								eventName = (string?)value.Value;
								break;
							case "Message":
								message = value.IsNull ? "" : (string)value.Value!;
								break;
						}
					}
				}
			}

			return (new ZLoggerMessageAttribute()
			{
				EventId = eventId,
				EventName = eventName,
				Level = level,
				Message = message,
				SkipEnabledCheck = skipEnabledCheck,
			}, setLogLevel);
		}

		MethodParameter[] GetMethodParameters(IMethodSymbol method, bool setLogLevel)
		{
			var result = new MethodParameter[method.Parameters.Length];

			var foundFirstLogger = false;
			var foundFirstLogLevel = false;
			var foundFirstException = false;

			if (setLogLevel)
			{
				// If LogLevel is set from Attribute, does not use LogLevel on method parameter.
				foundFirstLogLevel = true;
			}

			for (int i = 0; i < method.Parameters.Length; i++)
			{
				var p = method.Parameters[i];

				if (!foundFirstLogger)
				{
					var isLogger = p.Type.AllInterfaces.Concat(new[] { p.Type }).Any(x => SymbolEqualityComparer.Default.Equals(x, loggerSymbol));
					if (isLogger)
					{
						foundFirstLogger = true;
						result[i] = new MethodParameter
						{
							Symbol = p,
							IsFirstLogger = true,
						};
						continue;
					}
				}
				if (!foundFirstLogLevel)
				{
					var isLogLevel = SymbolEqualityComparer.Default.Equals(p.Type, logLevelSymbol);
					if (isLogLevel)
					{
						foundFirstLogLevel = true;
						result[i] = new MethodParameter
						{
							Symbol = p,
							IsFirstLogLevel = true,
						};
						continue;
					}
				}
				if (!foundFirstException)
				{
					var isException = SymbolEqualityComparer.Default.Equals(p.Type, exceptionSymbol);
					if (isException)
					{
						foundFirstException = true;
						result[i] = new MethodParameter
						{
							Symbol = p,
							IsFirstException = true,
						};
						continue;
					}
				}

				result[i] = new MethodParameter { Symbol = p };
			}

			return result;
		}

		bool Verify()
		{
			// LogLevel not found
			// must retrun void
			// missing ILogger
			// Must be static
			// Must be partial
			// generic is not supported
			// template has no corrresponding argument
			// argument has no corresponding template
			// invalid template
			// alow in, ref qualifier(out is fail)
			// primitives or IUtf8Formattbale
			// TODO:exception
			// TODO:logLevel from paramter?

			// check Duplicate EventId(allow -1)
			return true;
		}
	}
}
