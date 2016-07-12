/*
*   Untouched BdoTransformer (renamed to Transformer) by joarley
*   Many thanks!
*
*   ToDo: Clean up, remove unsafe code, find appropriate variable/function names
*/

using System;
using System.Linq;
using System.Security.Cryptography;

namespace Oasis.Network.Cryptography
{

    public unsafe class Transformer
    {
        private readonly byte[] _sessionKey = new byte[136];

        public Transformer()
        {
            fixed (byte* param1 = new byte[16])
            fixed (byte* param2 = new byte[8])
            fixed (byte* sessionKey = _sessionKey)
            {

                sub_C7EEF0(param1);
                sub_C7F300(param2);

                sub_D06AD0(param1, sessionKey);
                sub_D06F10(param2, sessionKey);
            }
        }

        public void Transform(ref byte[] data)
        {
            Transform(ref data, 0);
        }

        public void Transform(ref byte[] data, int offset)
        {
            fixed (byte* sessionKey = _sessionKey)
            fixed (byte* pdata = data)
                sub_D072B0(sessionKey, pdata + offset, (uint) (data.Length - offset));
        }

        public void UpdateServerInformation(byte[] seed)
        {
            var provider = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128,
                Padding = PaddingMode.None
            };

            using (provider)
            {
                var key = new byte[16];
                var state = new byte[64];
                seed = (new byte[] { 0, 0 }).Concat(seed).ToArray();

                fixed (byte* sessionKey = _sessionKey)
                fixed (byte* pkey = key)
                fixed (byte* pstate = state)
                fixed (byte* pseed = seed)
                {
                    sub_D3A9C0(pseed, pkey);

                    using (var transformer = provider.CreateDecryptor(key, new byte[16]))
                    {
                        byte[] res = transformer.TransformFinalBlock(seed, 64, 16);
                        Array.Copy(res, 0, state, 0, 16);

                        res = transformer.TransformFinalBlock(seed, 17, 16);
                        Array.Copy(res, 0, state, 16, 16);

                        res = transformer.TransformFinalBlock(seed, 40, 16);
                        Array.Copy(res, 0, state, 32, 16);

                        res = transformer.TransformFinalBlock(seed, 87, 16);
                        Array.Copy(res, 0, state, 48, 16);
                    }

                    sub_D06AD0(pstate, sessionKey);
                    sub_D06F10(pstate + 2, sessionKey);
                }
            }
        }

        private void sub_D3A9C0(byte* a1, byte* a2)
        {
            int v2; // ebp@1
            byte* v3 = null; // eax@4
            byte v4; // bl@20
            int v5; // esi@20
            sbyte v6; // al@22
            sbyte v7; // bl@22
            sbyte v8; // al@23
            sbyte v9; // dl@25
            sbyte v10; // cl@26
            byte v11; // al@29
            sbyte result; // al@31
            sbyte v13; // cl@32
            sbyte v14; // dl@34
            sbyte v15; // al@34
            sbyte v16; // cl@35
            int v17; // [sp+Ch] [bp-4h]@1

            v2 = 0;
            v17 = 16;
            do
            {
                if ((uint) (v2 - 1) > 0xE)
                {
                    v3 = a1 + 15;
                }
                else
                {
                    switch (v2)
                    {
                        case 1:
                            v3 = a1 + 3;
                            break;
                        case 2:
                            v3 = a1 + 62;
                            break;
                        case 3:
                            v3 = a1 + 5;
                            break;
                        case 4:
                            v3 = a1 + 39;
                            break;
                        case 5:
                            v3 = a1 + 37;
                            break;
                        case 6:
                            v3 = a1 + 33;
                            break;
                        case 7:
                            v3 = a1 + 59;
                            break;
                        case 8:
                            v3 = a1 + 10;
                            break;
                        case 9:
                            v3 = a1 + 7;
                            break;
                        case 10:
                            v3 = a1 + 104;
                            break;
                        case 11:
                            v3 = a1 + 82;
                            break;
                        case 12:
                            v3 = a1 + 38;
                            break;
                        case 13:
                            v3 = a1 + 13;
                            break;
                        case 14:
                            v3 = a1 + 84;
                            break;
                        case 15:
                            v3 = a1 + 12;
                            break;
                    }
                }
                v4 = *(byte*) v3;
                v5 = v2 % 3;
                if (v2 % 3 > 0)
                {
                    if (v2 % 3 == 1)
                    {
                        v8 = (sbyte) (v2 % 8);
                        if ((byte) v8 < 1u)
                            v8 = 1;
                        v9 = v8;
                        v6 = (sbyte) (v4 << v8);
                        v7 = (sbyte) (v4 >> (8 - v9));
                    }
                    else
                    {
                        v6 = (sbyte) (v4 >> 4);
                        v7 = (sbyte) (16 * v4);
                    }
                }
                else
                {
                    v10 = (sbyte) (v2 % 8);
                    if ((byte) v10 < 1u)
                        v10 = 1;
                    v6 = (sbyte) (v4 >> v10);
                    v7 = (sbyte) (v4 << (8 - v10));
                }
                v11 = *(byte*) sub_D3ABF0(a1, (sbyte) (v6 | v7));
                if (v5 > 0)
                {
                    if (v5 != 1)
                    {
                        result = (sbyte) ((v11 >> 4) | 16 * v11);
                        goto LABEL_39;
                    }
                    v13 = (sbyte) (v2 % 8);
                    if ((byte) v13 < 1u)
                        v13 = 1;
                    v14 = (sbyte) (v11 << v13);
                    v15 = (sbyte) (v11 >> (8 - v13));
                }
                else
                {
                    v16 = (sbyte) (v2 % 8);
                    if ((byte) v16 < 1u)
                        v16 = 1;
                    v14 = (sbyte) (v11 >> v16);
                    v15 = (sbyte) (v11 << (8 - v16));
                }
                result = (sbyte) (v14 | v15);
                LABEL_39:
                *(byte*) (a2 + v2++) = (byte) result;
                --v17;
            } while (v17 > 0);
        }

        private byte* sub_D3ABF0(byte* a1, sbyte a2)
        {
            byte* result;

            switch (a2)
            {
                case 1:
                    result = a1 + 81;
                    break;
                case 2:
                    result = a1 + 9;
                    break;
                case 3:
                    result = a1 + 107;
                    break;
                case 4:
                    result = a1 + 36;
                    break;
                case 5:
                    result = a1 + 106;
                    break;
                case 6:
                    result = a1 + 14;
                    break;
                case 7:
                    result = a1 + 11;
                    break;
                case 8:
                    result = a1 + 34;
                    break;
                case 9:
                    result = a1 + 56;
                    break;
                case 0xA:
                    result = a1 + 6;
                    break;
                case 0xB:
                    result = a1 + 60;
                    break;
                case 0xC:
                    result = a1 + 85;
                    break;
                case 0xD:
                    result = a1 + 35;
                    break;
                case 0xE:
                    result = a1 + 61;
                    break;
                case 0xF:
                    result = a1 + 16;
                    break;
                case 0x10:
                    result = a1 + 8;
                    break;
                case 0x11:
                    result = a1 + 63;
                    break;
                case 0x12:
                    result = a1 + 57;
                    break;
                case 0x13:
                    result = a1 + 2;
                    break;
                case 0x14:
                    result = a1 + 80;
                    break;
                case 0x15:
                    result = a1 + 105;
                    break;
                case 0x16:
                    result = a1 + 58;
                    break;
                case 0x17:
                    result = a1 + 86;
                    break;
                case 0x18:
                    result = a1 + 103;
                    break;
                case 0x19:
                    result = a1 + 83;
                    break;
                default:
                    result = a1 + 4;
                    break;
            }
            return result;
        }

        private byte sub_B8C620(int a2, byte[] initialKey)
        {
            byte result; // eax@2

            switch (a2)
            {
                case 1:
                    result = initialKey[80];
                    break;
                case 2:
                    result = initialKey[123];
                    break;
                case 3:
                    result = initialKey[9];
                    break;
                case 4:
                    result = initialKey[37];
                    break;
                case 5:
                    result = initialKey[10];
                    break;
                case 6:
                    result = initialKey[14];
                    break;
                case 7:
                    result = initialKey[32];
                    break;
                case 8:
                    result = initialKey[35];
                    break;
                case 9:
                    result = initialKey[73];
                    break;
                case 0xA:
                    result = initialKey[76];
                    break;
                case 0xB:
                    result = initialKey[11];
                    break;
                case 0xC:
                    result = initialKey[101];
                    break;
                case 0xD:
                    result = initialKey[2];
                    break;
                case 0xE:
                    result = initialKey[7];
                    break;
                case 0xF:
                    result = initialKey[33];
                    break;
                case 0x10:
                    result = initialKey[75];
                    break;
                case 0x11:
                    result = initialKey[103];
                    break;
                case 0x12:
                    result = initialKey[78];
                    break;
                case 0x13:
                    result = initialKey[36];
                    break;
                case 0x14:
                    result = initialKey[79];
                    break;
                case 0x15:
                    result = initialKey[120];
                    break;
                case 0x16:
                    result = initialKey[83];
                    break;
                case 0x17:
                    result = initialKey[102];
                    break;
                case 0x18:
                    result = initialKey[122];
                    break;
                case 0x19:
                    result = initialKey[81];
                    break;
                default:
                    result = initialKey[4];
                    break;
            }
            return result;
        }

        private uint __ROL__(uint value, int count)
        {
            const uint nbits = sizeof(uint) * 8;

            if (count > 0)
            {
                count = (int) (count % nbits);
                uint high = value >> (int) (nbits - count);
                if (-1 < 0) // signed value
                    high &= (uint) ~((-1 << count));
                value <<= count;
                value |= high;
            }
            else
            {
                count = (int) (-count % nbits);
                uint low = value << (int) (nbits - count);
                value >>= count;
                value |= low;
            }
            return value;
        }

        private uint __ROL4__(uint value, int count)
        {
            return __ROL__((uint) value, count);
        }

        private uint __ROR4__(uint value, int count)
        {
            return __ROL__((uint) value, -count);
        }

        private int sub_D072B0(byte* a1, byte* a3, uint a4)
        {
            byte* v4; // ebx@1
            byte* v5; // edi@1
            int result; // eax@1
            byte* v7; // esi@2
            byte* v8; // eax@3
            int v9; // ecx@3
            bool v10; // zf@3
            uint v17; // ecx@6
            uint v18; // [sp+Ch] [bp-14h]@2

            v4 = a3;
            v5 = a1;
            result = (int) a4;
            if (a4 >= 0x10)
            {
                v7 = v5 + 68;
                v18 = a4 >> 4;
                do
                {
                    sub_D068F0(v7);
                    a4 -= 16;
                    v8 = a3 + 16;
                    *(uint*) (v8 - 16) = *(uint*) v7 ^ *(uint*) v4 ^ (*(uint*) (v5 + 80) << 16) ^ *(ushort*) (v5 + 90);
                    v9 = (int) (*(uint*) (v4 + 4) ^ (*(uint*) (v5 + 88) << 16) ^ *(ushort*) (v5 + 98));
                    v4 += 16;
                    a3 += 16;
                    *(uint*) (v8 - 12) = *(uint*) (v5 + 76) ^ (uint) v9;
                    *(uint*) (v8 - 8) = *(uint*) (v5 + 84) ^ *(uint*) (v4 - 8) ^ (*(uint*) (v5 + 96) << 16) ^ *(ushort*) (v5 + 74);
                    v10 = v18-- == 1;
                    *(uint*) (v8 - 4) = *(uint*) (v5 + 92) ^ *(uint*) (v4 - 4) ^ (*(uint*) (v5 + 72) << 16) ^ *(ushort*) (v5 + 82);
                } while (!v10);
                result = (int) a4;
            }
            if (result > 0)
            {
                sub_D068F0(v5 + 68);
                fixed (int* teste = new int[33])
                {
                    teste[30] = *(int*) (v5 + 80);
                    teste[31] = *(int*) (v5 + 88) << 16;
                    teste[0] = (int) (*(uint*) (v5 + 68) ^ (*(uint*) (v5 + 88) >> 16) ^ (*(uint*) (v5 + 80) << 16));
                    teste[32] = *(int*) (v5 + 96);
                    teste[1] = (int) (*(uint*) (v5 + 76) ^ teste[31] ^ (*(uint*) (v5 + 96) >> 16));
                    teste[5] = *(int*) (v5 + 72);
                    teste[2] = *(int*) (v5 + 84) ^ (teste[32] << 16) ^ (teste[5] >> 16);
                    teste[3] = *(int*) (v5 + 92) ^ (teste[30] >> 16) ^ (teste[5] << 16);

                    v17 = 0;
                    if (a4 > 0)
                    {
                        do
                        {
                            result = ((sbyte*) teste)[v17];
                            ((byte*) a3)[v17] = (byte) (result ^ ((byte*) v4)[v17]);
                        } while (++v17 < a4);
                    }
                }
            }
            return result;
        }

        private int sub_D068F0(byte* a1)
        {
            uint v1; // eax@1
            int v2; // ecx@1
            bool v3; // cf@1
            int v4; // eax@1
            int v5; // ebx@1
            int v6 = 0; // eax@1
            int v7 = 0; // edx@1
            int v8; // edx@1
            uint v9; // eax@1
            int v10; // ebx@1
            int v11; // eax@1
            int v12; // edi@1
            int v13; // ebx@1
            int v14 = 0; // eax@1
            int v15 = 0; // edx@1
            int v16; // edx@1
            uint v17; // eax@1
            int v18; // ebx@1
            int v19; // ST20_4@1
            int v20; // ebp@1
            int v21 = 0; // eax@1
            int v22 = 0; // edx@1
            int v23; // ebx@1
            int v24; // edx@1
            int v25; // eax@1
            uint v26; // eax@1
            int v27; // ebp@1
            int v28; // eax@1
            int v29; // ebp@1
            int v30; // ecx@1
            int v31 = 0; // eax@1
            int v32 = 0; // edx@1
            int v33; // ST24_4@1
            uint v34; // eax@1
            int v35; // ebp@1
            int v36; // ST1C_4@1
            int v37 = 0; // eax@1
            int v38 = 0; // edx@1
            int v39; // edx@1
            uint v40; // eax@1
            int v41; // ebp@1
            int v42; // edx@1
            int v43; // ecx@1
            int v44; // eax@1
            int v45; // ebx@1
            int v46 = 0; // eax@1
            int v47 = 0; // edx@1
            int v48; // edx@1
            int v49; // eax@1
            int v50; // ST28_4@1
            uint v51; // eax@1
            int v52; // ebx@1
            int v53; // ST1C_4@1
            long v54; // ST08_8@1
            long v55; // ST00_8@1
            int v56 = 0; // eax@1
            int v57 = 0; // edx@1
            int v58; // edx@1
            int v59; // eax@1
            int v60; // ecx@1
            int v61; // ebx@1
            uint v62; // ebp@1
            uint v63; // ST24_4@1
            int v64; // ebp@1
            int v65 = 0; // eax@1
            int v66 = 0; // edx@1
            int v67; // edx@1
            int v68; // eax@1
            int result; // eax@1
            int v70; // ecx@1
            int v71; // ebx@1

            v1 = *(uint*) (a1 + 32);
            v2 = (int) v1 + *(int*) (a1 + 64);
            v3 = (uint) v2 < v1;
            v4 = v2 + *(int*) a1;
            *(int*) (a1 + 32) = v2;
            v5 = (v3 ? 1 : 0) - 749914925;
            ulong t = sub_1496830((uint) v4, (int) v4);
            v6 = (int) (t >> 32);
            v7 = (int) t;
            v8 = v6 ^ v7;
            v9 = *(uint*) (a1 + 36);
            v10 = (int) v9 + v5;
            v3 = (uint) v10 < v9;
            v11 = *(int*) (a1 + 4);
            *(int*) (a1 + 36) = v10;
            v12 = v8;
            v13 = (v3 ? 1 : 0) + 886263092;
            t = sub_1496830((uint) (*(int*) (a1 + 36) + v11), (uint) (*(int*) (a1 + 36) + v11));
            v14 = (int) (t >> 32);
            v15 = (int) t;
            v16 = v14 ^ v15;
            v17 = *(uint*) (a1 + 40);
            v18 = (int) v17 + v13;
            *(int*) (a1 + 40) = v18;
            v19 = v16;
            v20 = ((uint) v18 < v17 ? 1 : 0) + 1295307597;
            t = sub_1496830((uint) (*(int*) (a1 + 8) + v18), (uint) (*(int*) (a1 + 8) + v18));
            v21 = (int) (t >> 32);
            v22 = (int) t;
            v23 = v21 ^ v22;
            v24 = (int) __ROL4__((uint) v19, 16);
            v25 = (int) __ROL4__((uint) v12, 16);
            *(int*) (a1 + 8) = v23 + v24 + v25;
            v26 = *(uint*) (a1 + 44);
            v27 = (int) v26 + v20;
            v3 = (uint) v27 < v26;
            v28 = *(int*) (a1 + 12);
            *(int*) (a1 + 44) = v27;
            v29 = (v3 ? 1 : 0) - 749914925;
            t = sub_1496830((uint) (*(int*) (a1 + 44) + v28), (uint) (*(int*) (a1 + 44) + v28));
            v31 = (int) (t >> 32);
            v32 = (int) t;
            v30 = (int) __ROL4__((uint) v23, 8);
            v33 = v31 ^ v32;
            v34 = *(uint*) (a1 + 48);
            v35 = (int) v34 + v29;
            *(int*) (a1 + 12) = v19 + v33 + v30;
            *(int*) (a1 + 48) = v35;
            v36 = ((uint) v35 < v34 ? 1 : 0) + 886263092;
            t = sub_1496830((uint) (*(int*) (a1 + 16) + v35), (uint) (*(int*) (a1 + 16) + v35));
            v37 = (int) (t >> 32);
            v38 = (int) t;
            v39 = v37 ^ v38;
            v40 = *(uint*) (a1 + 52);
            v41 = v39;
            v42 = (int) __ROL4__((uint) v33, 16);
            v23 = (int) __ROL4__((uint) v23, 16);
            v43 = (int) v40 + v36;
            v3 = v40 + (uint) v36 < v40;
            v44 = *(int*) (a1 + 20);
            *(int*) (a1 + 16) = v41 + v42 + v23;
            *(int*) (a1 + 52) = v43;
            v45 = (v3 ? 1 : 0) + 1295307597;
            t = sub_1496830((uint) (v43 + v44), (uint) (v43 + v44));
            v46 = (int) (t >> 32);
            v47 = (int) t;
            v48 = v46 ^ v47;
            v49 = (int) __ROL4__((uint) v41, 8);
            v50 = v48;
            *(int*) (a1 + 20) = v33 + v48 + v49;
            v51 = *(uint*) (a1 + 56);
            v52 = (int) v51 + v45;
            v53 = ((uint) v52 < v51 ? 1 : 0) - 749914925;
            v54 = (uint) (v52 + *(int*) (a1 + 24));
            v55 = (uint) (v52 + *(int*) (a1 + 24));
            *(int*) (a1 + 56) = v52;
            t = sub_1496830(v55, v54);
            v56 = (int) (t >> 32);
            v57 = (int) t;
            v58 = v56 ^ v57;
            v59 = *(int*) (a1 + 28);
            v60 = (int) __ROL4__((uint) v50, 16);
            v41 = (int) __ROL4__((uint) v41, 16);
            v61 = v58;
            *(int*) (a1 + 24) = v58 + v60 + v41;
            v62 = *(uint*) (a1 + 60);
            v63 = v62;
            v64 = v53 + (int) v62;
            *(int*) (a1 + 60) = v64;
            t = sub_1496830((uint) (v64 + v59), (uint) (v64 + v59));
            v65 = (int) (t >> 32);
            v66 = (int) t;
            v67 = v65 ^ v66;
            v68 = (int) __ROL4__((uint) v61, 8);
            result = v50 + v67 + v68;
            v70 = (int) __ROL4__((uint) v67, 16);
            v61 = (int) __ROL4__((uint) v61, 16);
            v71 = v12 + v70 + v61;
            v12 = (int) __ROL4__((uint) v12, 8);
            *(int*) (a1 + 4) = v19 + v67 + v12;
            *(int*) a1 = v71;
            *(int*) (a1 + 28) = result;
            *(int*) (a1 + 64) = ((uint) v64 < v63 ? 1 : 0) + 1295307597;
            return result;
        }

        private void sub_C7EEF0(byte* a1)
        {
            int v2; // ebp@1
            int v3; // edx@2
            int v4; // edi@2
            int v5; // esi@6
            uint v6; // ecx@6
            int v7; // ebx@6
            int v8; // esi@6
            int v9; // edi@6
            int v10; // edx@6
            int v11; // edi@6
            int v12; // ecx@6
            int v13; // ebx@6
            int v14; // edi@6
            int v15; // esi@6
            int v16; // edx@6
            int v17; // esi@6
            uint v18; // ecx@6
            int v19; // edx@7
            int v20; // edi@7
            int v21; // esi@10
            int v22; // esi@11
            uint v23; // ecx@11
            int v24; // ebx@11
            int v25; // esi@11
            int v26; // edi@11
            int v27; // edx@11
            int v28; // edi@11
            int v29; // ecx@11
            int v30; // ebx@11
            int v31; // edi@11
            int v32; // esi@11
            int v33; // edx@11
            int v34; // esi@11
            uint v35; // ecx@11
            byte v36; // bl@11
            int v37; // edx@12
            int v38; // edi@12
            int v39; // esi@16
            uint v40; // ecx@16
            int v41; // ebx@16
            int v42; // esi@16
            int v43; // edi@16
            int v44; // edx@16
            int v45; // edi@16
            int v46; // ecx@16
            int v47; // ebx@16
            int v48; // edi@16
            int v49; // esi@16
            int v50; // edx@16
            int v51; // esi@16
            int v52; // ecx@16
            int v53; // esi@17
            int v54; // edi@17
            int v55; // edx@21
            uint v56; // ecx@21
            int v57; // ebx@21
            int v58; // edx@21
            int v59; // edi@21
            int v60; // esi@21
            int v61; // edi@21
            int v62; // ecx@21
            int v63; // ebx@21
            int v64; // edi@21
            int v65; // edx@21
            int v66; // esi@21
            int v67; // edx@21
            uint v68; // ecx@21
            int v69; // [sp+18h] [bp+0h]@1
            ushort v70; // [sp+1Ch] [bp+4h]@1
            sbyte a2 = 7;

            v2 = a2 & 3;
            v70 = 94;
            v69 = -998529222;
            (*((sbyte*) &(v70) + 1)) = -105;
            if (v2 > 0)
            {
                if (((ulong) (&v69) & 1) > 0)
                {
                    v3 = -558189263;
                    v4 = -1556757235;
                }
                else
                {
                    v3 = v70 - 558228013;

                    v4 = ((*((short*) &(v69) + 1)) << 16) + (ushort) v69 - 558228013;
                }
            }
            else
            {
                v3 = v70 - 558228013;
                v4 = v69 - 558228013;
            }
            v5 = (int) __ROL4__((uint) v3, 14);
            v6 = (uint) ((v3 ^ 0xDEBA1DD3) - v5);
            v7 = (int) __ROL4__(v6, 11);
            v8 = (int) ((v4 ^ v6) - v7);
            v9 = (int) __ROR4__((uint) v8, 7);
            v10 = (v8 ^ v3) - v9;
            v11 = (int) __ROL4__((uint) v10, 16);
            v12 = (int) ((v10 ^ v6) - v11);
            v13 = (int) __ROL4__((uint) v12, 4);
            v14 = (v8 ^ v12) - v13;
            v15 = (int) __ROL4__((uint) v14, 14);
            v16 = (v14 ^ v10) - v15;
            v17 = (int) __ROR4__((uint) v16, 8);
            v18 = (uint) ((v16 ^ v12) - v17);
            *(byte*) (a1 + 10) = (*((byte*) &(v18) + 1));
            *(byte*) (a1 + 8) = (*((byte*) &(v18) + 3));
            (*((byte*) &(v69))) = (*((byte*) &(v18) + 1));
            *(byte*) (a1 + 3) = (byte) (v18 >> 16);
            (*((byte*) &(v69) + 2)) = (byte) (v18 >> 16);
            *(byte*) (a1 + 7) = (byte) v18;
            (*((byte*) &(v69) + 1)) = (byte) v18;
            (*((byte*) &(v69) + 3)) = (byte) (v18 >> 16);
            (*((byte*) &(v70))) = (*((byte*) &(v18) + 3));
            (*((byte*) &(v70) + 1)) = (byte) v18;
            if (v2 > 0)
            {
                if (((ulong) &v69 & 1) > 0)
                {
                    v21 = (byte) v18 << 8;
                    v19 = (byte) v70 + v21 - 558228013;
                    v20 = (byte) v69 + v21 + ((*((byte*) &(v69) + 2)) << 16) + ((byte) (v18 >> 16) << 24) - 558228013;
                }
                else
                {
                    v19 = v70 - 558228013;
                    v20 = ((*((short*) &(v69) + 1)) << 16) + (ushort) v69 - 558228013;
                }
            }
            else
            {
                v19 = v70 - 558228013;
                v20 = v69 - 558228013;
            }
            v22 = (int) __ROL4__((uint) v19, 14);
            v23 = (uint) ((v19 ^ 0xDEBA1DD3) - v22);
            v24 = (int) __ROL4__(v23, 11);
            v25 = (int) ((v20 ^ v23) - v24);
            v26 = (int) __ROR4__((uint) v25, 7);
            v27 = (v25 ^ v19) - v26;
            v28 = (int) __ROL4__((uint) v27, 16);
            v29 = (int) ((v27 ^ v23) - v28);
            v30 = (int) __ROL4__((uint) v29, 4);
            v31 = (v25 ^ v29) - v30;
            v32 = (int) __ROL4__((uint) v31, 14);
            v33 = (v31 ^ v27) - v32;
            v34 = (int) __ROR4__((uint) v33, 8);
            v35 = (uint) ((v33 ^ v29) - v34);
            *(byte*) (a1 + 15) = (*((byte*) &(v35) + 1));
            (*((byte*) &(v69))) = (*((byte*) &(v35) + 1));
            (*((byte*) &(v69) + 1)) = *(byte*) (a1 + 7);
            (*((byte*) &(v33))) = *(byte*) (a1 + 3);
            *(byte*) (a1 + 4) = (byte) (v35 >> 16);
            (*((byte*) &(v69) + 2)) = (byte) v33;
            *(byte*) (a1 + 11) = (*((byte*) &(v35) + 3));
            v36 = *(byte*) (a1 + 8);
            *(byte*) (a1 + 9) = (byte) v35;
            (*((byte*) &(v69) + 3)) = (byte) v35;
            (*((byte*) &(v70))) = v36;
            (*((byte*) &(v70) + 1)) = (byte) (v35 >> 16);
            if (v2 > 0)
            {
                if ((((ulong) &v69) & 1) > 0)
                {
                    v37 = v36 + ((*((byte*) &(v70) + 1)) << 8) - 558228013;
                    v38 = (byte) v69 + ((*((byte*) &(v69) + 1)) << 8) + ((*((byte*) &(v69) + 2)) << 16) + ((byte) v35 << 24) - 558228013;
                }
                else
                {
                    v37 = v70 - 558228013;
                    v38 = ((*((short*) &(v69) + 1)) << 16) + (ushort) v69 - 558228013;
                }
            }
            else
            {
                v37 = v70 - 558228013;
                v38 = v69 - 558228013;
            }
            v39 = (int) __ROL4__((uint) v37, 14);
            v40 = (uint) ((v37 ^ 0xDEBA1DD3) - v39);
            v41 = (int) __ROL4__(v40, 11);
            v42 = (int) ((v38 ^ v40) - v41);
            v43 = (int) __ROR4__((uint) v42, 7);
            v44 = (v42 ^ v37) - v43;
            v45 = (int) __ROL4__((uint) v44, 16);
            v46 = (int) ((v44 ^ v40) - v45);
            v47 = (int) __ROL4__((uint) v46, 4);
            v48 = (v42 ^ v46) - v47;
            v49 = (int) __ROL4__((uint) v48, 14);
            v50 = (v48 ^ v44) - v49;
            v51 = (int) __ROR4__((uint) v50, 8);
            v52 = (v50 ^ v46) - v51;
            *(byte*) a1 = (byte) ((uint) v52 >> 16);
            *(byte*) (a1 + 6) = (*((byte*) &(v52) + 3));
            (*((byte*) &(v69))) = *(byte*) (a1 + 9);
            (*((byte*) &(v69) + 1)) = *(byte*) (a1 + 4);
            (*((byte*) &(v70))) = (*((byte*) &(v52) + 3));
            *(byte*) (a1 + 1) = (byte) v52;
            *(byte*) (a1 + 13) = (*((byte*) &(v52) + 1));
            (*((byte*) &(v69) + 2)) = (*((byte*) &(v52) + 1));
            (*((byte*) &(v69) + 3)) = (byte) v52;
            (*((byte*) &(v70) + 1)) = (byte) ((uint) v52 >> 16);
            if (v2 > 0)
            {
                if ((((ulong) &v69) & 1) > 0)
                {
                    v53 = (byte) v70 + ((*((byte*) &(v70) + 1)) << 8) - 558228013;
                    v54 = (byte) v69 + ((*((byte*) &(v69) + 1)) << 8) + ((*((byte*) &(v51) + 1)) << 16) + ((byte) v52 << 24) - 558228013;
                }
                else
                {
                    v53 = v70 - 558228013;
                    v54 = ((*((short*) &(v69) + 1)) << 16) + (ushort) v69 - 558228013;
                }
            }
            else
            {
                v53 = v70 - 558228013;
                v54 = v69 - 558228013;
            }
            v55 = (int) __ROL4__((uint) v53, 14);
            v56 = (uint) ((v53 ^ 0xDEBA1DD3) - v55);
            v57 = (int) __ROL4__(v56, 11);
            v58 = (int) ((v54 ^ v56) - v57);
            v59 = (int) __ROR4__((uint) v58, 7);
            v60 = (v58 ^ v53) - v59;
            v61 = (int) __ROL4__((uint) v60, 16);
            v62 = (int) ((v60 ^ v56) - v61);
            v63 = (int) __ROL4__((uint) v62, 4);
            v64 = (v58 ^ v62) - v63;
            v65 = (int) __ROL4__((uint) v64, 14);
            v66 = (v64 ^ v60) - v65;
            v67 = (int) __ROR4__((uint) v66, 8);
            v68 = (uint) ((v66 ^ v62) - v67);
            *(byte*) (a1 + 12) = (*((byte*) &(v68) + 1));
            *(byte*) (a1 + 5) = (byte) v68;
            *(byte*) (a1 + 2) = (byte) (v68 >> 16);
            *(byte*) (a1 + 14) = (*((byte*) &(v68) + 3));
        }

        private void sub_C7F300(byte* a2)
        {
            int v2; // ebp@1
            int v3; // ecx@2
            int v4; // esi@2
            int v5; // edx@6
            uint v6; // eax@6
            int v7; // ebx@6
            int v8; // edx@6
            int v9; // esi@6
            int v10; // ecx@6
            int v11; // esi@6
            int v12; // eax@6
            int v13; // ebx@6
            int v14; // esi@6
            int v15; // edx@6
            int v16; // ecx@6
            int v17; // edx@6
            int v18; // eax@6
            int v19; // ecx@7
            int v20; // esi@7
            int v21; // edx@11
            uint v22; // eax@11
            int v23; // ebx@11
            int v24; // edx@11
            int v25; // esi@11
            int v26; // ecx@11
            int v27; // esi@11
            int v28; // eax@11
            int v29; // ebx@11
            int v30; // esi@11
            int v31; // edx@11
            int v32; // ecx@11
            int v33; // edx@11
            uint v34; // eax@11
            uint v35; // ecx@11
            uint v36; // edx@11
            uint result; // eax@11
            int v38; // [sp+Ch] [bp+0h]@1
            ushort v39; // [sp+10h] [bp+4h]@1

            sbyte a1 = 7;
            v2 = a1 & 3;
            v39 = 109;
            v38 = -778258853;
            (*((byte*) &(v39) + 1)) = unchecked((byte) -124);
            if (v2 > 0)
            {
                if ((((ulong) &v38) & 1) > 0)
                {
                    v3 = -558194112;
                    v4 = -1336486866;
                }
                else
                {
                    v3 = v39 - 558228013;
                    v4 = ((*((short*) &(v38) + 1)) << 16) + (ushort) v38 - 558228013;
                }
            }
            else
            {
                v3 = v39 - 558228013;
                v4 = v38 - 558228013;
            }
            v5 = (int) __ROL4__((uint) v3, 14);
            v6 = (uint) ((v3 ^ 0xDEBA1DD3) - v5);
            v7 = (int) __ROL4__((uint) v6, 11);
            v8 = (int) ((v4 ^ v6) - v7);
            v9 = (int) __ROR4__((uint) v8, 7);
            v10 = (v8 ^ v3) - v9;
            v11 = (int) __ROL4__((uint) v10, 16);
            v12 = (int) ((v10 ^ v6) - v11);
            v13 = (int) __ROL4__((uint) v12, 4);
            v14 = (v8 ^ v12) - v13;
            v15 = (int) __ROL4__((uint) v14, 14);
            v16 = (v14 ^ v10) - v15;
            v17 = (int) __ROR4__((uint) v16, 8);
            v18 = (v16 ^ v12) - v17;
            *(byte*) (a2 + 1) = (byte) ((uint) v18 >> 16);
            *(byte*) (a2 + 5) = (byte) v18;
            *(byte*) (a2 + 2) = (*((byte*) &(v18) + 1));
            *(byte*) (a2 + 7) = (*((byte*) &(v18) + 3));
            (*((byte*) &(v38))) = (*((byte*) &(v18) + 1));
            (*((byte*) &(v38) + 1)) = unchecked((byte) -100);
            (*((byte*) &(v38) + 2)) = (*((byte*) &(v18) + 1));
            (*((byte*) &(v38) + 3)) = (byte) v18;
            (*((byte*) &(v39))) = 109;
            (*((byte*) &(v39) + 1)) = (*((byte*) &(v18) + 3));
            if (v2 > 0)
            {
                if ((((ulong) &v38) & 1) > 0)
                {
                    v19 = ((*((byte*) &(v18) + 3)) << 8) - 558227904;
                    v20 = (*((byte*) &(v18) + 1)) + ((*((byte*) &(v38) + 2)) << 16) + ((byte) v18 << 24) - 558228013 + 39936;
                }
                else
                {
                    v19 = v39 - 558228013;
                    v20 = ((*((short*) &(v38) + 1)) << 16) + (ushort) v38 - 558228013;
                }
            }
            else
            {
                v19 = v39 - 558228013;
                v20 = v38 - 558228013;
            }
            v21 = (int) __ROL4__((uint) v19, 14);
            v22 = (uint) ((v19 ^ 0xDEBA1DD3) - v21);
            v23 = (int) __ROL4__((uint) v22, 11);
            v24 = (int) ((v20 ^ v22) - v23);
            v25 = (int) __ROR4__((uint) v24, 7);
            v26 = (v24 ^ v19) - v25;
            v27 = (int) __ROL4__((uint) v26, 16);
            v28 = (int) ((v26 ^ v22) - v27);
            v29 = (int) __ROL4__((uint) v28, 4);
            v30 = (v24 ^ v28) - v29;
            v31 = (int) __ROL4__((uint) v30, 14);
            v32 = (v30 ^ v26) - v31;
            v33 = (int) __ROR4__((uint) v32, 8);
            v34 = (uint) ((v32 ^ v28) - v33);
            v35 = v34 >> 8;
            v36 = v34 >> 16;
            *(byte*) (a2 + 4) = (byte) v34;
            result = v34 >> 24;
            *(byte*) (a2 + 6) = (byte) v35;
            *(byte*) a2 = (byte) v36;
            *(byte*) (a2 + 3) = (byte) result;
        }

        private void sub_D06AD0(byte* a1, byte* a2)
        {
            byte* v2; // edi@1
            uint v3; // ecx@1
            long v4; // rax@1
            uint v5; // ST50_4@1
            int v6; // ST64_4@1
            int v7; // ST60_4@1
            int v8; // ebx@1
            int v9; // edi@1
            uint v10; // edi@1
            uint v11; // ST54_4@1
            uint v12; // edi@1
            uint v13; // ST48_4@1
            uint v14; // edi@1
            int v15; // ST1C_4@1
            uint v16; // eax@1
            int v17; // edx@1
            int v18; // ebx@3
            int v19; // eax@3
            int v20; // edx@3
            int v21; // edx@3
            int v22; // eax@3
            int v23; // ebx@3
            int v24; // edi@3
            int v25; // eax@3
            int v26; // edx@3
            int v27; // edx@3
            int v28; // ST18_4@3
            uint v29; // eax@3
            int v30; // ST48_4@3
            int v31; // eax@3
            int v32; // edx@3
            int v33; // edx@3
            int v34; // eax@3
            int v35; // ebx@3
            int v36; // ecx@3
            int v37; // eax@3
            int v38; // ST18_4@3
            int v39; // eax@3
            int v40; // edx@3
            int v41; // edx@3
            int v42; // eax@3
            int v43; // ST54_4@3
            int v44; // eax@3
            int v45; // ST18_4@3
            int v46; // eax@3
            int v47; // edx@3
            int v48; // edx@3
            int v49; // eax@3
            int v50; // ebx@3
            int v51; // ST58_4@3
            int v52; // eax@3
            int v53; // edx@3
            int v54; // edx@3
            int v55; // eax@3
            int v56; // ST5C_4@3
            int v57; // ST18_4@3
            int v58; // eax@3
            int v59; // edx@3
            int v60; // edx@3
            int v61; // ecx@3
            int v62; // eax@3
            int v63; // ebx@3
            int v64; // ST58_4@3
            int v65; // eax@3
            int v66; // edx@3
            int v67; // edx@3
            int v68; // eax@3
            int v69; // eax@3
            int v70; // eax@3
            int v71; // ecx@4
            int result; // eax@4
            int v73; // [sp+8h] [bp-50h]@1
            int v74; // [sp+Ch] [bp-4Ch]@1
            int v75; // [sp+10h] [bp-48h]@1
            int v76; // [sp+14h] [bp-44h]@1
            int v77; // [sp+18h] [bp-40h]@1
            uint v78; // [sp+1Ch] [bp-3Ch]@1
            int v79; // [sp+20h] [bp-38h]@1
            uint v80; // [sp+24h] [bp-34h]@1
            uint v81; // [sp+28h] [bp-30h]@1
            uint v82; // [sp+2Ch] [bp-2Ch]@1
            uint v83; // [sp+30h] [bp-28h]@1
            int v84; // [sp+34h] [bp-24h]@1
            long v85; // [sp+3Ch] [bp-1Ch]@1
            int v86; // [sp+50h] [bp-8h]@1
            int v87; // [sp+54h] [bp-4h]@3

            v2 = a1;
            v3 = *(uint*) a1;
            (*((int*) &(v4))) = *(int*) (a1 + 4);
            (*((int*) &(v4) + 1)) = *(int*) (v2 + 8);
            v5 = *(uint*) (v2 + 12);
            *(uint*) (a2 + 24) = v5;
            v6 = (int) (((*((uint*) &(v4) + 1)) >> 16) | (v5 << 16));
            *(int*) (a2 + 4) = v6;
            v7 = (int) ((v3 << 16) | (v5 >> 16));
            *(int*) (a2 + 12) = v7;
            *(int*) (a2 + 20) = (int) (v3 >> 16) | ((int) v4 << 16);
            v8 = (int) v5;
            *(int*) (a2 + 28) = (int) (v4 >> 16);
            v9 = (int) __ROL4__((*((uint*) &(v4) + 1)), 16);
            *(int*) (a2 + 32) = v9;
            v10 = __ROL4__((uint) v5, 16);
            v11 = v10;
            *(int*) (a2 + 40) = (int) v10;
            v12 = __ROL4__(v3, 16);
            v13 = v12;
            *(int*) (a2 + 48) = (int) v12;
            v14 = __ROL4__((uint) v4, 16);
            v83 = v14;
            *(int*) (a2 + 56) = (int) v14;
            *(int*) (a2 + 36) = (int) (v3 ^ (ushort) (v3 ^ v4));
            v80 = (uint) (v4 ^ (ushort) (v4 ^ (*((ushort*) &(v4) + 2))));
            *(int*) (a2 + 44) = (int) v80;
            v15 = (*((int*) &(v4) + 1)) ^ (ushort) ((*((short*) &(v4) + 2)) ^ v5);
            *(int*) (a2 + 52) = v15;
            *(int*) a2 = (int) v3;
            *(int*) (a2 + 8) = (int) v4;
            *(int*) (a2 + 16) = (*((int*) &(v4) + 1));
            *(int*) (a2 + 60) = v8 ^ (ushort) (v3 ^ v8);
            *(int*) (a2 + 64) = 1295307597;
            v85 = v4;
            v84 = v7;
            v79 = (int) (v4 >> 16);
            v78 = v11;
            v81 = v13;
            v76 = (int) (v3 ^ (ushort) (v3 ^ v4));
            v73 = v6;
            v75 = (int) ((v3 >> 16) | ((int) v4 << 16));
            v16 = *(uint*) (a2 + 32);
            v82 = (uint) v15;
            v77 = v8;
            v74 = v8 ^ (ushort) (v3 ^ v8);
            v17 = 1295307597;
            v86 = 4;
            while (true)
            {
                v87 = (int) (v17 + v16);
                v18 = (((((uint) v17 + v16) < v16) ? 1 : 0) - 749914925);
                ulong t = sub_1496830((uint) v3 + (uint) v87, (uint) v3 + (uint) v87);
                v19 = (int) (t >> 32);
                v20 = (int) t;
                v21 = v19 ^ v20;
                v22 = v18 + v76;
                v23 = -((uint) v18 + (uint) v76 < (uint) v76 ? 1 : 0);
                v76 = v22;
                v24 = v21;
                v23 = 886263092 - v23;
                t = sub_1496830((uint) (v73 + v22), (uint) (v73 + v22));
                v25 = (int) (t >> 32);
                v26 = (int) t;
                v27 = v25 ^ v26;
                v28 = ((uint) v23 + v78 < v78 ? 1 : 0) + 1295307597;
                v29 = (uint) (v23 + v78 + v85);
                v78 += (uint) v23;
                v30 = v27;
                t = sub_1496830(v29, v29);
                v31 = (int) (t >> 32);
                v32 = (int) t;
                v33 = v31 ^ v32;
                v34 = (int) __ROL4__((uint) v30, 16);
                v35 = v33;
                v36 = (int) __ROL4__((uint) v24, 16);
                (*((int*) &(v85))) = v33 + v34 + v36;
                v37 = (int) (v28 + (int) v80);
                v38 = ((uint) v28 + v80 < v80 ? 1 : 0) - 749914925;
                v80 = (uint) v37;
                t = sub_1496830((uint) (v84 + v37), (uint) (v84 + v37));
                v39 = (int) (t >> 32);
                v40 = (int) t;
                v41 = v39 ^ v40;
                v42 = (int) __ROL4__((uint) v35, 8);
                v43 = v41;
                v84 = v30 + v41 + v42;
                v44 = (int) (v38 + v81);
                v45 = ((uint) v38 + v81 < v81 ? 1 : 0) + 886263092;
                v81 = (uint) v44;
                t = sub_1496830((uint) ((*((int*) &(v85) + 1)) + v44), (uint) ((*((int*) &(v85) + 1)) + v44));
                v46 = (int) (t >> 32);
                v47 = (int) t;
                v48 = v46 ^ v47;
                v49 = (int) __ROL4__((uint) v43, 16);
                v35 = (int) __ROL4__((uint) v35, 16);
                (*((int*) &(v85) + 1)) = v48 + v49 + v35;
                v50 = -((uint) v45 + v82 < v82 ? 1 : 0);
                v82 += (uint) v45;
                v51 = v48;
                v50 = 1295307597 - v50;
                t = sub_1496830(v75 + v82, v75 + v82);
                v52 = (int) (t >> 32);
                v53 = (int) t;
                v54 = v52 ^ v53;
                v55 = (int) __ROL4__((uint) v51, 8);
                v56 = v54;
                v75 = v43 + v54 + v55;
                v57 = ((uint) v50 + v83 < v83 ? 1 : 0) - 749914925;
                v83 += (uint) v50;
                t = sub_1496830((uint) v77 + (uint) v83, (uint) v77 + (uint) v83);
                v58 = (int) (t >> 32);
                v59 = (int) t;
                v60 = v58 ^ v59;
                v61 = (int) __ROL4__((uint) v56, 16);
                v62 = (int) __ROL4__((uint) v51, 16);
                v63 = v60;
                v77 = v60 + v61 + v62;
                v64 = v74;
                v74 += v57;
                t = sub_1496830((uint) (v79 + v74), (uint) (v79 + v74));
                v65 = (int) (t >> 32);
                v66 = (int) t;
                v67 = v65 ^ v66;
                v68 = (int) __ROL4__((uint) v63, 8);
                v79 = v56 + v67 + v68;
                v69 = (int) __ROL4__((uint) v67, 16);
                v70 = v24 + v69;
                v63 = (int) __ROL4__((uint) v63, 16);
                v24 = (int) __ROL4__((uint) v24, 8);
                v3 = (uint) (v63 + v70);
                v73 = v30 + v67 + v24;
                v17 = ((uint) v74 < (uint) v64 ? 1 : 0) + 1295307597;
                --v86;
                if (v86 == 0)
                    break;
                v16 = (uint) v87;
            }
            *(int*) (a2 + 24) = v77;
            *(int*) (a2 + 4) = v73;
            *(int*) (a2 + 12) = v84;
            *(int*) (a2 + 16) = (*((int*) &(v85) + 1));
            *(int*) (a2 + 84) = (*((int*) &(v85) + 1));
            *(int*) (a2 + 20) = v75;
            *(int*) (a2 + 28) = v79;
            *(int*) (a2 + 32) = v87;
            *(int*) (a2 + 32) ^= (*((int*) &(v85) + 1));
            *(int*) (a2 + 100) = *(int*) (a2 + 32);
            *(int*) (a2 + 88) = v75;
            *(int*) (a2 + 8) = (int) (v85);
            *(int*) (a2 + 36) = v76 ^ v75;
            *(int*) (a2 + 104) = v76 ^ v75;
            *(int*) (a2 + 92) = v77;
            *(int*) (a2 + 76) = (int) (v85);
            *(int*) (a2 + 40) = (int) (v78 ^ v77);
            *(int*) (a2 + 108) = (int) (v78 ^ v77);
            *(int*) (a2 + 96) = v79;
            *(int*) a2 = (int) (v3);
            *(int*) (a2 + 44) = (int) (v80 ^ v79);
            *(int*) (a2 + 112) = (int) (v80 ^ v79);
            *(int*) (a2 + 72) = v73;
            *(int*) (a2 + 68) = (int) v3;
            v71 = (int) (v81 ^ v3);
            *(int*) (a2 + 52) = (int) (v82 ^ v73);
            *(int*) (a2 + 120) = (int) (v82 ^ v73);
            *(int*) (a2 + 80) = v84;
            result = v74 ^ v84;
            *(int*) (a2 + 56) = (int) (v83 ^ v85);
            *(int*) (a2 + 124) = (int) (v83 ^ v85);
            *(int*) (a2 + 64) = v17;
            *(int*) (a2 + 48) = v71;
            *(int*) (a2 + 116) = v71;
            *(int*) (a2 + 60) = v74 ^ v84;
            *(int*) (a2 + 128) = v74 ^ v84;
            *(int*) (a2 + 132) = v17;
        }

        private ulong sub_1496830(long a1, long a2)
        {
            ulong result; // rax@2

            if (((*((int*) &(a1) + 1)) | (*((int*) &(a2) + 1))) > 0)
                result = (ulong) (a2 * a1);
            else
                result = (uint) a2 * (ulong) (uint) a1;
            return result;
        }

        private void sub_D06F10(byte* a1, byte* a2)
        {
            int v2; // edx@1
            int v3; // ebx@1
            int v4; // edi@1
            uint v5; // eax@1
            int v6; // ecx@1
            int v7; // ebx@1
            int v8; // edx@1
            int v9; // eax@1
            int v10; // edx@1
            int v11; // ecx@1
            int v12; // eax@1
            int v13; // edx@1
            int v14; // eax@1
            int v15; // ecx@1
            int v16; // edx@1
            int v17; // eax@1
            int v18; // ecx@1
            int v19; // edx@1
            int v20; // eax@1
            uint v21; // ecx@1
            int v22; // ebx@2
            int v23; // eax@2
            int v24; // edx@2
            int v25; // edx@2
            int v26; // eax@2
            int v27; // ebx@2
            int v28; // edi@2
            int v29; // eax@2
            int v30; // edx@2
            int v31; // ST20_4@2
            int v32; // ST18_4@2
            int v33; // eax@2
            int v34; // eax@2
            int v35; // edx@2
            int v36; // edx@2
            int v37; // eax@2
            int v38; // ebx@2
            int v39; // ecx@2
            int v40; // eax@2
            int v41; // ST18_4@2
            int v42; // eax@2
            int v43; // edx@2
            int v44; // edx@2
            int v45; // eax@2
            int v46; // ST54_4@2
            int v47; // eax@2
            int v48; // ST18_4@2
            int v49; // eax@2
            int v50; // edx@2
            int v51; // edx@2
            int v52; // eax@2
            int v53; // ebx@2
            int v54; // ST58_4@2
            int v55; // eax@2
            int v56; // edx@2
            int v57; // edx@2
            int v58; // eax@2
            int v59; // ST5C_4@2
            int v60; // ST18_4@2
            int v61; // eax@2
            int v62; // eax@2
            int v63; // edx@2
            int v64; // edx@2
            int v65; // ecx@2
            int v66; // eax@2
            int v67; // ebx@2
            uint v68; // ST58_4@2
            int v69; // eax@2
            int v70; // edx@2
            int v71; // edx@2
            int v72; // eax@2
            int v73; // eax@2
            int v74; // ebx@2
            int v75; // edi@2
            bool v76; // zf@2
            int result; // eax@3
            uint v78; // [sp+Ch] [bp-4Ch]@1
            int v79; // [sp+10h] [bp-48h]@1
            uint v80; // [sp+14h] [bp-44h]@1
            uint v81; // [sp+18h] [bp-40h]@1
            int v82; // [sp+1Ch] [bp-3Ch]@1
            uint v83; // [sp+20h] [bp-38h]@1
            int v84; // [sp+24h] [bp-34h]@1
            uint v85; // [sp+28h] [bp-30h]@1
            int v86; // [sp+2Ch] [bp-2Ch]@1
            uint v87; // [sp+30h] [bp-28h]@1
            int v88; // [sp+34h] [bp-24h]@1
            uint v89; // [sp+38h] [bp-20h]@1
            int v90; // [sp+3Ch] [bp-1Ch]@1
            int v91; // [sp+40h] [bp-18h]@1
            int v92; // [sp+50h] [bp-8h]@1
            int v93; // [sp+54h] [bp-4h]@2

            v2 = *(int*) a1;
            v3 = (ushort) *(int*) a1;
            v4 = *(int*) (a1 + 4);
            v5 = (uint) (v4 & 0xFFFF0000 | (*(uint*) a1 >> 16));
            v6 = v3 | (v4 << 16);
            *(int*) (a2 + 100) = v2 ^ *(int*) (a2 + 32);
            *(int*) (a2 + 104) = (int) (v5 ^ *(int*) (a2 + 36));
            *(int*) (a2 + 108) = v4 ^ *(int*) (a2 + 40);
            *(int*) (a2 + 112) = v6 ^ *(int*) (a2 + 44);
            v7 = v2 ^ *(int*) (a2 + 48);
            v8 = (int) (v5 ^ *(int*) (a2 + 52));
            v9 = *(int*) (a2 + 56);
            *(int*) (a2 + 120) = v8;
            v10 = v6 ^ *(int*) (a2 + 60);
            v11 = *(int*) (a2 + 4);
            *(int*) (a2 + 124) = v4 ^ v9;
            v12 = *(int*) a2;
            *(int*) (a2 + 128) = v10;
            v13 = *(int*) (a2 + 8);
            *(int*) (a2 + 68) = v12;
            v14 = *(int*) (a2 + 12);
            *(int*) (a2 + 72) = v11;
            v15 = *(int*) (a2 + 16);
            *(int*) (a2 + 76) = v13;
            v16 = *(int*) (a2 + 20);
            *(int*) (a2 + 80) = v14;
            v17 = *(int*) (a2 + 24);
            *(int*) (a2 + 84) = v15;
            v18 = *(int*) (a2 + 28);
            *(int*) (a2 + 88) = v16;
            v19 = *(int*) (a2 + 64);
            *(int*) (a2 + 92) = v17;
            *(int*) (a2 + 116) = v7;
            *(int*) (a2 + 96) = v18;
            *(int*) (a2 + 132) = v19;
            v20 = v19;
            v80 = *(uint*) (a2 + 104);
            v79 = *(int*) (a2 + 72);
            v81 = *(uint*) (a2 + 108);
            v21 = *(uint*) (a2 + 100);
            v82 = *(int*) (a2 + 76);
            v83 = *(uint*) (a2 + 112);
            v84 = *(int*) (a2 + 80);
            v86 = *(int*) (a2 + 84);
            v87 = *(uint*) (a2 + 120);
            v88 = *(int*) (a2 + 88);
            v89 = *(uint*) (a2 + 124);
            v90 = *(int*) (a2 + 92);
            v85 = (uint) v7;
            v78 = *(uint*) (a2 + 128);
            v91 = *(int*) (a2 + 96);
            v92 = 4;
            do
            {
                v93 = (int) (v20 + v21);
                v22 = ((uint) v20 + v21 < v21 ? 1 : 0) - 749914925;
                ulong t = sub_1496830((int) (v93 + *(uint*) (a2 + 68)), (int) (v93 + *(uint*) (a2 + 68)));
                v23 = (int) (t >> 32);
                v24 = (int) t;
                v25 = v23 ^ v24;
                v26 = (int) ((uint) v22 + v80);
                v27 = -((uint) v22 + v80 < v80 ? 1 : 0);
                v80 = (uint) v26;
                v28 = v25;
                v27 = 886263092 - v27;
                t = sub_1496830((int) (v79 + v26), (int) (v79 + v26));
                v29 = (int) (t >> 32);
                v30 = (int) t;
                v31 = v29 ^ v30;
                v32 = ((uint) v27 + v81 < v81 ? 1 : 0) + 1295307597;
                v33 = (int) (v27 + v81 + v82);
                v81 += (uint) v27;
                t = sub_1496830((int) v33, (int) v33);
                v34 = (int) (t >> 32);
                v35 = (int) t;
                v36 = v34 ^ v35;
                v37 = (int) __ROL4__((uint) v31, 16);
                v38 = v36;
                v39 = (int) __ROL4__((uint) v28, 16);
                v82 = v36 + v37 + v39;
                v40 = (int) (v32 + v83);
                v41 = ((uint) v32 + v83 < v83 ? 1 : 0) - 749914925;
                v83 = (uint) v40;
                t = sub_1496830((int) (v84 + v40), (int) (v84 + v40));
                v42 = (int) (t >> 32);
                v43 = (int) t;
                v44 = v42 ^ v43;
                v45 = (int) __ROL4__((uint) v38, 8);
                v46 = v44;
                v84 = v31 + v44 + v45;
                v47 = (int) (v41 + v85);
                v48 = ((uint) v41 + v85 < v85 ? 1 : 0) + 886263092;
                v85 = (uint) v47;
                t = sub_1496830((int) (v86 + v47), (int) (v86 + v47));
                v49 = (int) (t >> 32);
                v50 = (int) t;
                v51 = v49 ^ v50;
                v52 = (int) __ROL4__((uint) v46, 16);
                v38 = (int) __ROL4__((uint) v38, 16);
                v86 = v51 + v52 + v38;
                v53 = -((uint) v48 + v87 < v87 ? 1 : 0);
                v87 += (uint) v48;
                v54 = v51;
                v53 = 1295307597 - v53;
                t = sub_1496830(v88 + (int) v87, v88 + (int) v87);
                v55 = (int) (t >> 32);
                v56 = (int) t;
                v57 = v55 ^ v56;
                v58 = (int) __ROL4__((uint) v54, 8);
                v59 = v57;
                v88 = v46 + v57 + v58;
                v60 = ((uint) v53 + v89 < v89 ? 1 : 0) - 749914925;
                v61 = (int) (v53 + v89 + v90);
                v89 += (uint) v53;
                t = sub_1496830((int) v61, (int) v61);
                v62 = (int) (t >> 32);
                v63 = (int) t;
                v64 = v62 ^ v63;
                v65 = (int) __ROL4__((uint) v59, 16);
                v66 = (int) __ROL4__((uint) v54, 16);
                v67 = v64;
                v90 = v64 + v65 + v66;
                v68 = v78;
                v78 += (uint) v60;
                t = sub_1496830(v91 + v78, v91 + v78);
                v69 = (int) (t >> 32);
                v70 = (int) t;
                v71 = v69 ^ v70;
                v72 = (int) __ROL4__((uint) v67, 8);
                v21 = (uint) v93;
                v91 = v59 + v71 + v72;
                v73 = (int) __ROL4__((uint) v71, 16);
                v67 = (int) __ROL4__((uint) v67, 16);
                v74 = v28 + v73 + v67;
                v28 = (int) __ROL4__((uint) v28, 8);
                v75 = v31 + v71 + v28;
                *(int*) (a2 + 68) = v74;
                v20 = (v78 < v68 ? 1 : 0) + 1295307597;
                v76 = v92-- == 1;
                v79 = v75;
            } while (!v76);
            *(int*) (a2 + 132) = v20;
            *(int*) (a2 + 100) = v93;
            *(int*) (a2 + 104) = (int) v80;
            *(int*) (a2 + 72) = v75;
            *(int*) (a2 + 108) = (int) v81;
            *(int*) (a2 + 76) = v82;
            *(int*) (a2 + 112) = (int) v83;
            *(int*) (a2 + 80) = v84;
            *(int*) (a2 + 116) = (int) v85;
            *(int*) (a2 + 84) = v86;
            *(int*) (a2 + 120) = (int) v87;
            *(int*) (a2 + 88) = v88;
            *(int*) (a2 + 124) = (int) v89;
            result = v91;
            *(int*) (a2 + 92) = v90;
            *(int*) (a2 + 128) = (int) v78;
            *(int*) (a2 + 96) = v91;
        }
    }

}