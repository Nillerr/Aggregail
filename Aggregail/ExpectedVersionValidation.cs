using System;

namespace Aggregail
{
    public static class ExpectedVersionValidation
    {
        public static void ValidateExpectedVersion(long expectedVersion)
        {
            if (expectedVersion < ExpectedVersion.StreamExists || expectedVersion == -3L)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expectedVersion), expectedVersion,
                    $"The expected version must be either a value found in {nameof(ExpectedVersion)}, or a value " +
                    $"greater than or equal to 0."
                );
            }
        }

        public static long StartingVersion(
            long expectedVersion,
            long? currentVersion,
            string stream
        )
        {
            return currentVersion == null
                ? StartingVersionWhenNoStream(stream, expectedVersion)
                : StartingVersionWhenStreamExists(stream, expectedVersion, currentVersion.Value);
        }

        private static long StartingVersionWhenNoStream(string stream, long expectedVersion)
        {
            if (expectedVersion == ExpectedVersion.StreamExists)
            {
                throw ExpectedStreamToExist(stream);
            }

            if (expectedVersion >= 0L)
            {
                throw ExpectedStreamToExist(stream, expectedVersion);
            }

            return 0L;
        }

        public static WrongExpectedVersionException ExpectedStreamToExist(string stream)
        {
            return new WrongExpectedVersionException(
                $"Expected stream `{stream}` to exist, but the stream did not exist.",
                ExpectedVersion.StreamExists, null
            );
        }

        public static WrongExpectedVersionException ExpectedStreamToExist(string stream, long expectedVersion)
        {
            return new WrongExpectedVersionException(
                $"Expected stream `{stream}` to be at version {expectedVersion}, but the stream did not exist.",
                expectedVersion, null
            );
        }

        private static long StartingVersionWhenStreamExists(
            string stream,
            long expectedVersion,
            long currentVersion
        )
        {
            if (expectedVersion == ExpectedVersion.NoStream)
            {
                throw new WrongExpectedVersionException(
                    $"Expected stream `{stream}` to not exist, but the stream did exist at version {currentVersion}.",
                    expectedVersion, currentVersion
                );
            }

            if (expectedVersion == ExpectedVersion.Any)
            {
                return currentVersion + 1L;
            }

            if (expectedVersion <= currentVersion)
            {
                // The EventId of each event in the stream starting from expectedVersion are compared to those in the
                // write operation.
                return expectedVersion + 1L;
            }

            // expectedVersion > currentVersion
            throw UnexpectedVersion(stream, expectedVersion, currentVersion);
        }

        public static WrongExpectedVersionException UnexpectedVersion(string stream, long expectedVersion, long currentVersion)
        {
            return new WrongExpectedVersionException(
                $"Expected stream `{stream}` to be at version {expectedVersion}, but was at version {currentVersion}.",
                expectedVersion, currentVersion
            );
        }
    }
}