using Microsoft.Extensions.Logging;
using Moq;

namespace MeAgendaAi.Common
{
    public static class LoggerMockExtensions
    {
        public static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock, LogLevel logLevel,
            string message, string failMessage = null!) =>
                loggerMock.VerifyLog(logLevel, message, Times.Once(), failMessage);

        public static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock, Exception exception,
            LogLevel logLevel, string message, string failMessage = null!) =>
                loggerMock.VerifyLog(logLevel, exception, message, Times.Once(), failMessage);

        public static void VerifyLog<T, E>(
            this Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message, Times times, string failMessage = null!) where E : Exception =>
                loggerMock.Verify(verify => verify.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString() == message),
                    It.IsAny<E>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), times, failMessage);

        private static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock, LogLevel logLevel, Exception exception,
            string message, Times times, string failMessage = null!) =>
                loggerMock.Verify(verify => verify.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, _) => o.ToString() == message),
                    exception,
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                    times,
                    failMessage);

        private static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock, LogLevel logLevel, string message,
            Times times, string failMessage = null!) => loggerMock.Verify(verify => verify.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString() == message),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                times,
                failMessage);
    }
}