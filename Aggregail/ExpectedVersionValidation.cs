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
                throw new WrongExpectedVersionException(
                    $"Expected stream `{stream}` to exist, but the stream did not exist.",
                    expectedVersion, null
                );
            }

            if (expectedVersion == ExpectedVersion.StreamExists)
            {
                throw new WrongExpectedVersionException(
                    $"Expected stream `{stream}` to exist, but the stream did not exist.",
                    expectedVersion, null
                );
            }

            if (expectedVersion >= 0L)
            {
                throw new WrongExpectedVersionException(
                    $"Expected stream `{stream}` to be at version {expectedVersion}, but the stream did not exist.",
                    expectedVersion, null
                );
            }

            return 0L;
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
            throw new WrongExpectedVersionException(
                $"Expected stream `{stream}` to be at version {expectedVersion}, but was at version {currentVersion}.",
                expectedVersion, currentVersion
            );
        }
    }
}