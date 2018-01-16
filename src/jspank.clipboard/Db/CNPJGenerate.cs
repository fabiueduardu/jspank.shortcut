using System;
using System.Linq;

namespace jspank.clipboard.Db
{
    public class CNPJGenerate
    {
        public static decimal Get
        {
            get
            {
                var result = new Int64[13];
                var rand = new Random();
                int i;
                for (i = 1; i <= 8; i++)
                {
                    result[i] = (rand.Next()) % 9;
                }
                result[9] = 0;
                result[10] = 0;
                result[11] = 0;
                result[12] = (rand.Next()) % 9;

                var soma1 = ((result[1] * 5) +
                      (result[2] * 4) +
                      (result[3] * 3) +
                      (result[4] * 2) +
                      (result[5] * 9) +
                      (result[6] * 8) +
                      (result[7] * 7) +
                      (result[8] * 6) +
                      (result[9] * 5) +
                      (result[10] * 4) +
                      (result[11] * 3) +
                      (result[12] * 2));
                var part1 = (soma1 / 11);
                var part2 = (part1 * 11);
                var part3 = (soma1 - part2);
                var dig1 = (11 - part3);
                if (dig1 > 9) dig1 = 0;

                var soma2 = ((result[1] * 6) +
                       (result[2] * 5) +
                       (result[3] * 4) +
                       (result[4] * 3) +
                       (result[5] * 2) +
                       (result[6] * 9) +
                       (result[7] * 8) +
                       (result[8] * 7) +
                       (result[9] * 6) +
                       (result[10] * 5) +
                       (result[11] * 4) +
                       (result[12] * 3) +
                       (dig1 * 2));
                var part5 = (soma2 / 11);
                var part6 = (part5 * 11);
                var part7 = (soma2 - part6);
                var dig2 = (11 - part7);
                if (dig2 > 9) dig2 = 0;
           
                var result_nums = string.Join("", result.Select((v, idx) => new { v, idx }).Where(a => a.idx > 0).Select(a => a.v));
                return decimal.Parse(string.Concat(result_nums, dig1, dig2));
            }
        }
    }
}
