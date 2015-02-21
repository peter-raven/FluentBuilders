using System;
using System.IO;
using System.Text;

namespace BuildBuddy.Core
{
    /// <summary>
    /// Generators for random stuff
    /// </summary>
    public static class Generate
    {
        /// <summary>
        /// Generates a string containing the notorious Lorem Ipsum text, truncated to the length specified.
        /// </summary>
        /// <param name="length">Length of the string</param>
        /// <returns></returns>
        public static string LoremIpsum(int length)
        {
            string lorem = loremIpsum;
            while (length > lorem.Length)
                lorem += loremIpsum;
            return lorem.Substring(0, length);
        }

        /// <summary>
        /// Generates a completely random string.
        /// </summary>
        /// <param name="length">Length of the string.</param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            string randomString = string.Empty;

            while (randomString.Length <= length)
            {
                randomString += Path.GetRandomFileName();
                randomString = randomString.Replace(".", string.Empty);
            }

            return randomString.Substring(0, length);
        }

        /// <summary>
        /// Generates a string containing only digits.
        /// </summary>
        /// <param name="length">Length of the string.</param>
        /// <returns></returns>
        public static string RandomNumberString(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(RandomNumber(10));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="max">Maximum value to generate.</param>
        /// <returns></returns>
        public static int RandomNumber(int max)
        {
            var rnd = new Random();
            return rnd.Next(max + 1);
        }

        private static string loremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean vel elit massa, ac accumsan eros. Etiam mollis tempor magna ac dictum. Pellentesque eget ligula ante, et dictum neque. Fusce faucibus commodo lacus ut accumsan. Etiam condimentum leo et neque ornare adipiscing. Curabitur vel consectetur metus. Vivamus dignissim luctus mauris. Morbi auctor ante vel arcu tincidunt eget dapibus ante malesuada. Aenean at ligula ut elit varius adipiscing. Etiam sit amet massa a arcu egestas vehicula. In sed mi mauris. In tempor laoreet ante, id tempus magna fermentum id. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.
                                         Aliquam at lectus nisi. Nullam eu nisi a augue egestas cursus. Proin sit amet lectus in ligula facilisis tempor vel eu justo. Mauris mollis ultricies nibh, sed facilisis nisl ullamcorper at. Donec tristique purus sit amet enim rutrum cursus. Proin et enim sem. Donec at dui nisi. Nulla accumsan, purus at consectetur egestas, ligula dui molestie nulla, ac blandit arcu enim ac quam. Sed interdum odio ac tortor hendrerit commodo. Donec tincidunt metus et nisl bibendum adipiscing. Maecenas iaculis, lacus at viverra ultrices, nulla lectus gravida orci, ac blandit magna libero vel velit. Phasellus varius feugiat neque a commodo.
                                         Curabitur accumsan placerat adipiscing. Quisque dignissim enim vitae mauris condimentum cursus. Praesent cursus augue eu nulla pretium ac pretium nisl sodales. Nullam sit amet porta nunc. Nunc tincidunt rutrum pharetra. Nam faucibus lacus quis ligula commodo ac malesuada felis venenatis. Aenean laoreet erat commodo risus tristique at lacinia mauris suscipit. Curabitur a urna nulla. Nunc semper odio sit amet purus facilisis in imperdiet lorem adipiscing. Aenean non dui felis. Integer suscipit, nisl a tempus dapibus, erat nisl bibendum justo, molestie fringilla lectus nisi ac nisi. Nam vel ultrices justo.
                                         Phasellus eros arcu, placerat et ullamcorper ut, iaculis nec enim. Duis magna elit, malesuada in luctus sed, dignissim non nulla. Praesent cursus varius commodo. Morbi lobortis turpis eu enim pharetra fermentum. Duis vitae justo justo, vel auctor risus. Etiam justo augue, volutpat nec venenatis sed, interdum in sem. Suspendisse et dolor lacus. Nullam vel tellus sed mi bibendum vulputate nec vel arcu. Nulla facilisi. Duis quis ipsum arcu, sodales imperdiet ligula. Aliquam suscipit est non lorem egestas hendrerit. Donec tortor dui, rutrum vitae venenatis ut, tempor sit amet lacus. Pellentesque ut aliquet erat.
                                         Proin sed libero sem. Praesent consectetur sodales dictum. Vestibulum sodales mattis gravida. Vestibulum felis tortor, ullamcorper nec consectetur sed, condimentum ut risus. Maecenas facilisis volutpat ligula, id vulputate turpis hendrerit vel. Nunc eu ullamcorper justo. In id porta quam. Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

    }
}