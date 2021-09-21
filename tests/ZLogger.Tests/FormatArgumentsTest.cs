using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ZLogger.Tests
{
    public class FormatArgumentsTest
    {
        [Fact]
        public void CheckFormatArguments()
        {
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger("mytest");

            logger.ZLogTrace("{}", 1);
            logger.ZLogTrace("{} {}", 1, 2);
            logger.ZLogTrace("{} {} {}", 1, 2, 3);
            logger.ZLogTrace("{} {} {} {}", 1, 2, 3, 4);
            logger.ZLogTrace("{} {} {} {} {}", 1, 2, 3, 4, 5);
            logger.ZLogTrace("{} {} {} {} {} {}", 1, 2, 3, 4, 5, 6);
            logger.ZLogTrace("{} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7);
            logger.ZLogTrace("{} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            logger.ZLogTrace("{} {} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            logger.ZLogDebug("{}", 1);
            logger.ZLogDebug("{} {}", 1, 2);
            logger.ZLogDebug("{} {} {}", 1, 2, 3);
            logger.ZLogDebug("{} {} {} {}", 1, 2, 3, 4);
            logger.ZLogDebug("{} {} {} {} {}", 1, 2, 3, 4, 5);
            logger.ZLogDebug("{} {} {} {} {} {}", 1, 2, 3, 4, 5, 6);
            logger.ZLogDebug("{} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7);
            logger.ZLogDebug("{} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            logger.ZLogDebug("{} {} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            logger.ZLogInformation("{}", 1);
            logger.ZLogInformation("{} {}", 1, 2);
            logger.ZLogInformation("{} {} {}", 1, 2, 3);
            logger.ZLogInformation("{} {} {} {}", 1, 2, 3, 4);
            logger.ZLogInformation("{} {} {} {} {}", 1, 2, 3, 4, 5);
            logger.ZLogInformation("{} {} {} {} {} {}", 1, 2, 3, 4, 5, 6);
            logger.ZLogInformation("{} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7);
            logger.ZLogInformation("{} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            logger.ZLogInformation("{} {} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            logger.ZLogWarning("{}", 1);
            logger.ZLogWarning("{} {}", 1, 2);
            logger.ZLogWarning("{} {} {}", 1, 2, 3);
            logger.ZLogWarning("{} {} {} {}", 1, 2, 3, 4);
            logger.ZLogWarning("{} {} {} {} {}", 1, 2, 3, 4, 5);
            logger.ZLogWarning("{} {} {} {} {} {}", 1, 2, 3, 4, 5, 6);
            logger.ZLogWarning("{} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7);
            logger.ZLogWarning("{} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            logger.ZLogWarning("{} {} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
            logger.ZLogError("{}", 1);
            logger.ZLogError("{} {}", 1, 2);
            logger.ZLogError("{} {} {}", 1, 2, 3);
            logger.ZLogError("{} {} {} {}", 1, 2, 3, 4);
            logger.ZLogError("{} {} {} {} {}", 1, 2, 3, 4, 5);
            logger.ZLogError("{} {} {} {} {} {}", 1, 2, 3, 4, 5, 6);
            logger.ZLogError("{} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7);
            logger.ZLogError("{} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8);
            logger.ZLogError("{} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
            logger.ZLogError("{} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            logger.ZLogError("{} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            logger.ZLogError("{} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            logger.ZLogError("{} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            logger.ZLogError("{} {} {} {} {} {} {} {} {} {} {} {} {} {}", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14);
        }
    }
}
