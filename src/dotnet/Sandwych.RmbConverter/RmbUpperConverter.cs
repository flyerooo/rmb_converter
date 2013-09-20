﻿/** 大写人民币金额转换代码
 * 作者：李维 <oldrev@gmail.com>
 * 版权所有 (c) 2013 昆明维智众源企业管理咨询有限公司。保留所有权利。
 * 本代码基于 BSD License 授权。
 * */


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Sandwych.RmbConverter {
    public static class RmbUpperConverter {

        private static readonly Char[] RmbDigits = {
            '零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖' };

        private static readonly string[] SectionChars = {
             string.Empty, "拾", "佰", "仟", "万" };

        public static string ToRmbUpper(this decimal price) {
            if (price < 0M || price >= 9999999999999999.99M) {
                throw new ArgumentOutOfRangeException("price");
            }

            price = Math.Round(price, 2);
            var sb = new StringBuilder();

            var wanyiPart = (long)price / 1000000000000L;
            var yiPart = ((long)price % 1000000000000L) / 100000000L;
            var wanPart = ((long)price % 100000000L) / 10000L;
            var qianPart = (long)(price % 10000L);
            var integerPart = (long)price;
            var decPart = (long)(price * 100) % 100;

            //处理万亿以上的部分
            if (price >= 1000000000000M) {
                ParseInteger(sb, wanyiPart);
                sb.Append("万");
            }

            //处理亿到千亿的部分
            if (price >= 100000000M) {
                if (price >= 1000000000000M && yiPart > 0 && yiPart <= 999) {
                    sb.Append("零");
                }
                ParseInteger(sb, yiPart);
                sb.Append("亿");
            }

            //处理万的部分
            if (price >= 10000M) {
                if (price >= 100000000M && wanPart > 0 && wanPart <= 999) {
                    sb.Append("零");
                }
                ParseInteger(sb, wanPart);
                sb.Append("万");
            }

            //处理千及以后的部分
            if (price >= 10000M && qianPart > 0 && qianPart <= 999) {
                sb.Append("零");
            }
            if (qianPart > 0) {
                ParseInteger(sb, qianPart);
            }

            if (integerPart > 0) {
                sb.Append("元");
            }

            //处理小数
            if (decPart > 0) {
                ParseDecimal(sb, decPart);
            }
            else {
                sb.Append("整");
            }

            return sb.ToString();
        }

        private static void ParseDecimal(StringBuilder sb, long decPart) {
            Debug.Assert(decPart > 0 && decPart <= 99);
            var jiao = decPart / 10;
            var fen = decPart % 10;
            if (jiao > 0) {
                sb.Append(RmbDigits[jiao]);
                sb.Append("角");
            }
            if (jiao == 0 && fen > 0) {
                sb.Append("零");
            }
            if (fen > 0) {
                sb.Append(RmbDigits[fen]);
                sb.Append("分");
            }
            else {
                sb.Append("整");
            }
        }

        private static void ParseInteger(StringBuilder sb, long integer) {
            Debug.Assert(integer > 0 && integer <= 9999);
            int nDigits = (int)Math.Floor(Math.Log10(integer)) + 1;
            var zeroCount = 0;
            for (var i = 0; i < nDigits; i++) {
                var factor = (long)Math.Pow(10, nDigits - 1 - i);
                var digit = integer / factor;

                if (digit != 0) {
                    if (zeroCount > 0) {
                        sb.Append("零");
                    }
                    sb.Append(RmbDigits[digit]);
                    sb.Append(SectionChars[nDigits - i - 1]);
                    zeroCount = 0;
                }
                else {
                    if (i < nDigits) {
                        zeroCount++;
                    }
                }
                integer -= integer / factor * factor;
            }
        }

    }


}
