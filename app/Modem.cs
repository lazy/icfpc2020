using System;
using System.Collections.Generic;

namespace app {
    public static class Modem {
        public static bool[] Modulate(Value value) {
            var stack = new Stack<Value>(new[] {value});
            var bits = new List<bool>();
            while (stack.Count > 0) {
                var topValue = stack.Pop();
                switch (value) {
                    case Nil nil:
                        bits.AddRange(new[] {false, false});
                        break;
                    case Integer integer:
                        ModulateInteger(integer.Val, bits);
                        break;
                    case Pair pair:
                        bits.AddRange(new[] {true, true});
                        stack.Push(pair.Second);
                        stack.Push(pair.First);
                        break;
                    default:
                        throw new Exception($"can't modulate {value}");
                }
            }
            return bits.ToArray();
        }

        public static Value Demodulate(bool[] bits) {
            var instructions = new List<Value>();
            var cursor = 0;
            while (cursor < bits.Length) {
                if (cursor + 1 >= bits.Length) {
                    throw new ArgumentException("bits ended unexpectedly");
                }

                var b0 = bits[0];
                var b1 = bits[1];

                if (b0 == b1) {
                    if (b0) {
                        instructions.Add(new Pair());
                    } else {
                        instructions.Add(Nil.Instance);
                    }
                } else {
                    instructions.Add(new Integer {Val = DemodulateInteger(bits, cursor)});
                }
            }

            var stack = new Stack<Value>();

            for (var idx = instructions.Count - 1; idx >= 0; --idx) {
                var instr = instructions[idx];
                switch (instr) {
                    case Pair pair:
                        pair.First = stack.Pop();
                        pair.Second = stack.Pop();
                        stack.Push(pair);
                    break;
                    default:
                        stack.Push(instr);
                    break;
                }
            }

            if (stack.Count != 1) {
                throw new ArgumentException("expected to have exactly one element in the stack after processing bits");
            }
            return stack.Pop();
        }

        private static void ModulateInteger(long num, List<bool> bits) {
            bits.AddRange(num >= 0 ? new[] {false, true} : new[] {true, false});
            num = Math.Abs(num);

            var tmpBits = new List<bool>();
            while (num > 0) {
                tmpBits.Add((num & 1) != 0);
                num >>= 1;
            }
            while (tmpBits.Count % 4 != 0) {
                tmpBits.Add(false);
            }
            tmpBits.Reverse();

            var widthInNibbles = tmpBits.Count / 4;
            for (var idx = 0; idx < widthInNibbles; ++idx) {
                bits.Add(true);
            }
            bits.Add(false);

            bits.AddRange(tmpBits);
        }

        private static long DemodulateInteger(bool[] bits, int cursor) {
            if (bits.Length < 3) {
                throw new ArgumentException("bits is expected to have at least 3 elements");
            }

            var isNegative = bits[cursor++];
            var complement = bits[cursor++];
            if (isNegative == complement) {
                throw new ArgumentException("bits should start with either 01 or 10");
            }

            var widthInNibbles = 0;
            while (cursor < bits.Length && bits[cursor]) {
                ++widthInNibbles;
                ++cursor;
            }

            var realLength = bits.Length - cursor;
            if (realLength != widthInNibbles * 4) {
                throw new ArgumentException($"The number is expected to have exactly {widthInNibbles} nibbles");
            }

            var result = 0L;
            for (var idx = bits.Length - 1; idx >= cursor; --idx) {
                if (bits[idx]) {
                    result |= 1;
                }
                result <<= 1;
            }
            return result;
        }
    }
}