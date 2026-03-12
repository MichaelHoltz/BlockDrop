using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlockDrop.Classes.BlockDrop
{
    /// <summary>
    /// 100 procedurally-defined alien symbol glyphs stored as normalised 0–1
    /// vector data. Organised into four script families:
    ///   A (0–24)  Circular Glyphic — circle-framed symbols with internal geometry
    ///   B (25–49) Runic Angular   — sharp hooks, serifs, and angular strokes
    ///   C (50–74) Flowing Organic  — curved tendrils and vine-like forms
    ///   D (75–99) Technical Geo    — circuit-trace and nested-frame patterns
    /// </summary>
    internal static class AlienGlyphs
    {
        public const int Count = 100;

        /// <summary>Lines[glyph][polyline] = flat {x1,y1, x2,y2, …} pairs</summary>
        private static readonly float[][][] _lines = new float[Count][][];

        /// <summary>Circles[glyph][i] = {cx, cy, radius} — stroked outlines</summary>
        private static readonly float[][][] _circles = new float[Count][][];

        /// <summary>Dots[glyph][i] = {cx, cy, radius} — filled circles</summary>
        private static readonly float[][][] _dots = new float[Count][][];

        static AlienGlyphs()
        {
            DefineGlyphs();
        }

        /// <summary>
        /// Draws the glyph at <paramref name="index"/> into <paramref name="cell"/>
        /// using the specified <paramref name="color"/> and <paramref name="penWidth"/>.
        /// </summary>
        public static void Draw(int index, Graphics g, RectangleF cell, Color color, float penWidth)
        {
            float pad = cell.Width * 0.12f;
            float x0 = cell.X + pad;
            float y0 = cell.Y + pad;
            float sz = cell.Width - 2f * pad;
            if (sz <= 0) return;

            using (var pen = new Pen(color, penWidth))
            using (var brush = new SolidBrush(color))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                var lines = _lines[index];
                if (lines != null)
                {
                    foreach (var seg in lines)
                    {
                        for (int i = 0; i + 3 < seg.Length; i += 2)
                        {
                            g.DrawLine(pen,
                                x0 + seg[i] * sz, y0 + seg[i + 1] * sz,
                                x0 + seg[i + 2] * sz, y0 + seg[i + 3] * sz);
                        }
                    }
                }

                var circles = _circles[index];
                if (circles != null)
                {
                    foreach (var c in circles)
                    {
                        float r = c[2] * sz;
                        g.DrawEllipse(pen, x0 + c[0] * sz - r, y0 + c[1] * sz - r, r * 2, r * 2);
                    }
                }

                var dots = _dots[index];
                if (dots != null)
                {
                    foreach (var d in dots)
                    {
                        float r = d[2] * sz;
                        g.FillEllipse(brush, x0 + d[0] * sz - r, y0 + d[1] * sz - r, r * 2, r * 2);
                    }
                }
            }
        }

        /// <summary>Shorthand float-array builder.</summary>
        private static float[] F(params float[] v) => v;

        private static void DefineGlyphs()
        {
            // ═══════════════════════════════════════════════════════════════
            //  SCRIPT A (0–24): CIRCULAR GLYPHIC
            //  Circle-framed symbols with detailed internal structure.
            // ═══════════════════════════════════════════════════════════════

            // 0: Circle + asymmetric fork from top + left tick + accent dot
            _circles[0] = new[] { F(.5f, .5f, .42f) };
            _lines[0] = new[] {
                F(.5f, .12f, .5f, .52f, .24f, .82f),
                F(.5f, .52f, .78f, .72f),
                F(.2f, .35f, .35f, .35f),
                F(.78f, .72f, .85f, .78f)
            };
            _dots[0] = new[] { F(.74f, .2f, .05f) };

            // 1: Circle + internal zigzag + right vertical + bar + 2 dots
            _circles[1] = new[] { F(.5f, .5f, .42f) };
            _lines[1] = new[] {
                F(.2f, .22f, .45f, .35f, .2f, .5f, .45f, .65f, .2f, .78f),
                F(.68f, .25f, .68f, .75f),
                F(.58f, .5f, .78f, .5f),
                F(.25f, .15f, .32f, .1f)
            };
            _dots[1] = new[] { F(.5f, .12f, .04f), F(.5f, .88f, .04f) };

            // 2: Concentric circles + 4 radial connectors + diagonal accent
            _circles[2] = new[] { F(.5f, .5f, .42f), F(.5f, .5f, .2f) };
            _lines[2] = new[] {
                F(.5f, .08f, .5f, .3f), F(.5f, .7f, .5f, .92f),
                F(.08f, .5f, .3f, .5f), F(.7f, .5f, .92f, .5f),
                F(.68f, .12f, .8f, .06f), F(.15f, .82f, .08f, .9f)
            };
            _dots[2] = new[] { F(.5f, .5f, .05f) };

            // 3: Circle + internal staircase + 3 accent dots
            _circles[3] = new[] { F(.5f, .5f, .42f) };
            _lines[3] = new[] {
                F(.2f, .25f, .35f, .25f, .35f, .42f, .55f, .42f, .55f, .58f, .7f, .58f, .7f, .75f),
                F(.3f, .7f, .2f, .8f), F(.78f, .3f, .82f, .2f)
            };
            _dots[3] = new[] { F(.25f, .15f, .04f), F(.75f, .15f, .04f), F(.5f, .85f, .04f) };

            // 4: Circle + flowing S inside + center dot + ticks
            _circles[4] = new[] { F(.5f, .5f, .42f) };
            _lines[4] = new[] {
                F(.3f, .15f, .65f, .3f, .35f, .5f, .65f, .7f, .3f, .85f),
                F(.15f, .5f, .25f, .45f), F(.75f, .55f, .85f, .5f)
            };
            _dots[4] = new[] { F(.5f, .5f, .05f), F(.72f, .18f, .04f) };

            // 5: Circle + inner triangle + vertical extending below + top dot
            _circles[5] = new[] { F(.5f, .45f, .38f) };
            _lines[5] = new[] {
                F(.5f, .15f, .75f, .7f, .25f, .7f, .5f, .15f),
                F(.5f, .7f, .5f, .95f),
                F(.25f, .7f, .2f, .78f), F(.75f, .7f, .8f, .78f)
            };
            _dots[5] = new[] { F(.5f, .05f, .04f), F(.5f, .45f, .04f) };

            // 6: Circle + 3 unequal horizontal bars + 2 right dots
            _circles[6] = new[] { F(.5f, .5f, .42f) };
            _lines[6] = new[] {
                F(.18f, .3f, .65f, .3f),
                F(.25f, .5f, .82f, .5f),
                F(.18f, .7f, .55f, .7f),
                F(.4f, .3f, .35f, .2f), F(.6f, .5f, .65f, .4f)
            };
            _dots[6] = new[] { F(.8f, .3f, .04f), F(.8f, .7f, .04f) };

            // 7: Circle + angular spiral + accent dots
            _circles[7] = new[] { F(.5f, .5f, .42f) };
            _lines[7] = new[] {
                F(.2f, .2f, .78f, .2f, .78f, .78f, .2f, .78f, .2f, .38f, .62f, .38f, .62f, .62f, .38f, .62f, .38f, .48f, .52f, .48f)
            };
            _dots[7] = new[] { F(.52f, .48f, .04f), F(.15f, .12f, .04f) };

            // 8: Circle + internal lightning bolt + side ticks
            _circles[8] = new[] { F(.5f, .5f, .42f) };
            _lines[8] = new[] {
                F(.58f, .12f, .35f, .42f, .62f, .48f, .38f, .88f),
                F(.2f, .4f, .3f, .35f), F(.7f, .65f, .8f, .6f),
                F(.72f, .2f, .78f, .15f)
            };
            _dots[8] = new[] { F(.25f, .75f, .04f) };

            // 9: Circle + V-top + horizontal bar + pendant below + dot
            _circles[9] = new[] { F(.5f, .5f, .42f) };
            _lines[9] = new[] {
                F(.2f, .18f, .5f, .42f, .8f, .18f),
                F(.2f, .58f, .8f, .58f),
                F(.5f, .58f, .5f, .82f),
                F(.4f, .82f, .6f, .82f), F(.5f, .82f, .5f, .9f)
            };
            _dots[9] = new[] { F(.5f, .5f, .04f) };

            // 10: Circle + offset cross in upper-right + diagonal + accent
            _circles[10] = new[] { F(.5f, .5f, .42f) };
            _lines[10] = new[] {
                F(.55f, .2f, .55f, .55f), F(.38f, .35f, .75f, .35f),
                F(.2f, .65f, .45f, .8f),
                F(.7f, .6f, .82f, .72f), F(.2f, .2f, .28f, .15f)
            };
            _dots[10] = new[] { F(.75f, .75f, .04f), F(.25f, .82f, .04f) };

            // 11: Circle + internal diamond + vertical through + dots
            _circles[11] = new[] { F(.5f, .5f, .42f) };
            _lines[11] = new[] {
                F(.5f, .2f, .7f, .5f, .5f, .8f, .3f, .5f, .5f, .2f),
                F(.5f, .1f, .5f, .9f),
                F(.3f, .2f, .22f, .15f), F(.7f, .8f, .78f, .85f)
            };
            _dots[11] = new[] { F(.15f, .5f, .04f), F(.85f, .5f, .04f) };

            // 12: Circle + horizontal bar + 2 angled strokes below + tip dots
            _circles[12] = new[] { F(.5f, .5f, .42f) };
            _lines[12] = new[] {
                F(.18f, .4f, .82f, .4f),
                F(.35f, .4f, .2f, .78f), F(.65f, .4f, .8f, .78f),
                F(.5f, .4f, .5f, .2f),
                F(.2f, .78f, .15f, .85f), F(.8f, .78f, .85f, .85f)
            };
            _dots[12] = new[] { F(.5f, .15f, .04f), F(.2f, .82f, .04f), F(.8f, .82f, .04f) };

            // 13: Circle + inner semicircle-top + vertical + bottom hook + dot
            _circles[13] = new[] { F(.5f, .5f, .42f), F(.5f, .3f, .18f) };
            _lines[13] = new[] {
                F(.5f, .48f, .5f, .82f),
                F(.5f, .82f, .35f, .88f),
                F(.68f, .3f, .78f, .22f), F(.32f, .3f, .22f, .22f)
            };
            _dots[13] = new[] { F(.5f, .3f, .04f), F(.78f, .78f, .04f) };

            // 14: Circle + X with one arm extended + 3 dots
            _circles[14] = new[] { F(.5f, .5f, .42f) };
            _lines[14] = new[] {
                F(.22f, .22f, .78f, .78f), F(.78f, .22f, .22f, .78f),
                F(.78f, .78f, .9f, .92f),
                F(.15f, .5f, .22f, .5f), F(.78f, .5f, .85f, .5f)
            };
            _dots[14] = new[] { F(.5f, .12f, .04f), F(.12f, .5f, .04f), F(.88f, .5f, .04f) };

            // 15: Double circle + angular Z bridge + accent
            _circles[15] = new[] { F(.5f, .5f, .42f), F(.5f, .5f, .22f) };
            _lines[15] = new[] {
                F(.3f, .3f, .72f, .3f, .28f, .7f, .7f, .7f),
                F(.82f, .18f, .9f, .12f)
            };
            _dots[15] = new[] { F(.18f, .82f, .04f), F(.82f, .82f, .04f) };

            // 16: Circle + offset hash in lower-left + upper-right dot cluster
            _circles[16] = new[] { F(.5f, .5f, .42f) };
            _lines[16] = new[] {
                F(.2f, .45f, .2f, .82f), F(.38f, .45f, .38f, .82f),
                F(.12f, .55f, .45f, .55f), F(.12f, .72f, .45f, .72f),
                F(.6f, .25f, .72f, .18f), F(.72f, .25f, .8f, .18f)
            };
            _dots[16] = new[] { F(.65f, .2f, .04f), F(.75f, .2f, .04f), F(.7f, .3f, .04f) };

            // 17: Circle + archway + 2 pillar legs + keystone dot
            _circles[17] = new[] { F(.5f, .5f, .42f) };
            _lines[17] = new[] {
                F(.22f, .72f, .22f, .35f, .35f, .2f, .65f, .2f, .78f, .35f, .78f, .72f),
                F(.5f, .2f, .5f, .12f),
                F(.22f, .72f, .18f, .78f), F(.78f, .72f, .82f, .78f)
            };
            _dots[17] = new[] { F(.5f, .2f, .05f), F(.5f, .55f, .04f) };

            // 18: Circle + branching tree inside + accent dots
            _circles[18] = new[] { F(.5f, .5f, .42f) };
            _lines[18] = new[] {
                F(.5f, .85f, .5f, .45f),
                F(.5f, .45f, .25f, .2f), F(.5f, .45f, .75f, .2f),
                F(.25f, .2f, .15f, .12f), F(.25f, .2f, .32f, .12f),
                F(.75f, .2f, .68f, .12f), F(.75f, .2f, .85f, .12f),
                F(.5f, .62f, .35f, .55f), F(.5f, .62f, .65f, .55f)
            };
            _dots[18] = new[] { F(.5f, .88f, .04f) };

            // 19: Circle + angular omega inside + base dots
            _circles[19] = new[] { F(.5f, .5f, .42f) };
            _lines[19] = new[] {
                F(.18f, .75f, .28f, .75f, .22f, .45f, .35f, .22f, .65f, .22f, .78f, .45f, .72f, .75f, .82f, .75f),
                F(.4f, .22f, .4f, .15f), F(.6f, .22f, .6f, .15f)
            };
            _dots[19] = new[] { F(.28f, .8f, .04f), F(.5f, .8f, .04f), F(.72f, .8f, .04f) };

            // 20: Circle + vertical + 3 asymmetric crossbars + dot
            _circles[20] = new[] { F(.5f, .5f, .42f) };
            _lines[20] = new[] {
                F(.5f, .12f, .5f, .88f),
                F(.28f, .28f, .62f, .28f),
                F(.22f, .5f, .72f, .5f),
                F(.35f, .72f, .58f, .72f),
                F(.62f, .28f, .68f, .22f)
            };
            _dots[20] = new[] { F(.22f, .72f, .04f), F(.78f, .38f, .04f) };

            // 21: Circle + hourglass + 4 corner dots
            _circles[21] = new[] { F(.5f, .5f, .42f) };
            _lines[21] = new[] {
                F(.22f, .2f, .78f, .2f, .5f, .5f, .78f, .8f, .22f, .8f, .5f, .5f, .22f, .2f),
                F(.22f, .2f, .15f, .15f), F(.78f, .2f, .85f, .15f),
                F(.22f, .8f, .15f, .85f), F(.78f, .8f, .85f, .85f)
            };
            _dots[21] = new[] { F(.15f, .15f, .04f), F(.85f, .15f, .04f), F(.15f, .85f, .04f), F(.85f, .85f, .04f) };

            // 22: Circle + curved arrow (polyline) + inner dot + tip
            _circles[22] = new[] { F(.5f, .5f, .42f) };
            _lines[22] = new[] {
                F(.72f, .25f, .6f, .2f, .35f, .25f, .25f, .42f, .3f, .6f, .45f, .72f, .65f, .72f),
                F(.65f, .72f, .72f, .65f), F(.65f, .72f, .7f, .8f),
                F(.75f, .2f, .82f, .15f)
            };
            _dots[22] = new[] { F(.5f, .48f, .05f), F(.72f, .25f, .04f) };

            // 23: Circle + 2 opposing arcs inside + center dot + ticks
            _circles[23] = new[] { F(.5f, .5f, .42f), F(.5f, .28f, .15f), F(.5f, .72f, .15f) };
            _lines[23] = new[] {
                F(.5f, .13f, .5f, .08f), F(.5f, .87f, .5f, .92f),
                F(.15f, .5f, .22f, .45f), F(.85f, .5f, .78f, .55f)
            };
            _dots[23] = new[] { F(.5f, .5f, .05f), F(.2f, .2f, .04f), F(.8f, .8f, .04f) };

            // 24: Circle + 6-spoke star + small inner circle + outer dots
            _circles[24] = new[] { F(.5f, .5f, .42f), F(.5f, .5f, .1f) };
            _lines[24] = new[] {
                F(.5f, .12f, .5f, .88f),
                F(.17f, .3f, .83f, .7f),
                F(.83f, .3f, .17f, .7f),
                F(.12f, .5f, .18f, .48f), F(.88f, .5f, .82f, .52f)
            };
            _dots[24] = new[] { F(.5f, .05f, .04f), F(.5f, .95f, .04f), F(.12f, .26f, .04f), F(.88f, .74f, .04f) };

            // ═══════════════════════════════════════════════════════════════
            //  SCRIPT B (25–49): RUNIC ANGULAR
            //  Sharp angles, hooks, serifs, and asymmetric strokes.
            // ═══════════════════════════════════════════════════════════════

            // 25: Vertical shaft + forked top + hooked base + serifs + dots
            _lines[25] = new[] {
                F(.5f, .15f, .5f, .85f),
                F(.5f, .15f, .25f, 0f), F(.5f, .15f, .75f, 0f),
                F(.5f, .85f, .35f, .95f),
                F(.25f, 0f, .2f, .08f), F(.75f, 0f, .8f, .08f),
                F(.35f, .45f, .5f, .45f)
            };
            _dots[25] = new[] { F(.65f, .45f, .04f), F(.35f, .95f, .04f) };

            // 26: Two parallel verticals + 3 diagonals + serifs
            _lines[26] = new[] {
                F(.25f, .05f, .25f, .95f), F(.75f, .05f, .75f, .95f),
                F(.25f, .25f, .75f, .38f),
                F(.25f, .5f, .75f, .5f),
                F(.25f, .75f, .75f, .62f),
                F(.25f, .05f, .18f, .05f), F(.75f, .05f, .82f, .05f),
                F(.25f, .95f, .18f, .95f), F(.75f, .95f, .82f, .95f)
            };
            _dots[26] = new[] { F(.5f, .12f, .04f) };

            // 27: Angular N + top spike + base serif + accent dots
            _lines[27] = new[] {
                F(.2f, .92f, .2f, .08f, .8f, .72f, .8f, .08f),
                F(.2f, .08f, .2f, 0f), F(.8f, .08f, .85f, 0f),
                F(.2f, .92f, .12f, .95f), F(.8f, .72f, .88f, .72f),
                F(.35f, .42f, .42f, .38f)
            };
            _dots[27] = new[] { F(.5f, .5f, .04f), F(.85f, .92f, .04f) };

            // 28: Double chevron + center vertical + base T + accent
            _lines[28] = new[] {
                F(.15f, .3f, .5f, .08f, .85f, .3f),
                F(.22f, .48f, .5f, .3f, .78f, .48f),
                F(.5f, .3f, .5f, .82f),
                F(.3f, .82f, .7f, .82f),
                F(.5f, .82f, .5f, .95f),
                F(.15f, .3f, .08f, .32f), F(.85f, .3f, .92f, .32f)
            };
            _dots[28] = new[] { F(.5f, .95f, .04f) };

            // 29: Zigzag column + side branches + tip dots
            _lines[29] = new[] {
                F(.3f, .05f, .7f, .2f, .3f, .4f, .7f, .6f, .3f, .8f, .7f, .95f),
                F(.7f, .2f, .82f, .15f), F(.3f, .4f, .18f, .35f),
                F(.7f, .6f, .82f, .55f), F(.3f, .8f, .18f, .75f)
            };
            _dots[29] = new[] { F(.82f, .15f, .04f), F(.18f, .35f, .04f), F(.82f, .55f, .04f), F(.18f, .75f, .04f) };

            // 30: Angular U-frame + diagonal cross + dot
            _lines[30] = new[] {
                F(.15f, .08f, .15f, .78f, .5f, .92f, .85f, .78f, .85f, .08f),
                F(.28f, .28f, .72f, .65f), F(.72f, .28f, .28f, .65f),
                F(.15f, .08f, .08f, .05f), F(.85f, .08f, .92f, .05f)
            };
            _dots[30] = new[] { F(.5f, .47f, .05f), F(.5f, .85f, .04f) };

            // 31: Staircase + connecting diagonal + accent dots
            _lines[31] = new[] {
                F(.15f, .1f, .15f, .3f, .38f, .3f, .38f, .55f, .62f, .55f, .62f, .75f, .85f, .75f, .85f, .92f),
                F(.15f, .1f, .85f, .92f),
                F(.15f, .1f, .1f, .05f), F(.85f, .92f, .9f, .95f)
            };
            _dots[31] = new[] { F(.38f, .3f, .04f), F(.62f, .55f, .04f) };

            // 32: Fork-and-join + crossing + accent dots
            _lines[32] = new[] {
                F(.5f, .05f, .2f, .25f, .2f, .5f, .5f, .7f),
                F(.5f, .05f, .8f, .25f, .8f, .5f, .5f, .7f),
                F(.5f, .7f, .5f, .95f),
                F(.2f, .38f, .8f, .38f),
                F(.5f, .95f, .42f, .98f), F(.5f, .95f, .58f, .98f)
            };
            _dots[32] = new[] { F(.5f, .05f, .04f), F(.5f, .38f, .04f) };

            // 33: Angular F-rune + bottom kick + side tick + dots
            _lines[33] = new[] {
                F(.25f, .05f, .25f, .95f),
                F(.25f, .05f, .8f, .05f),
                F(.25f, .42f, .65f, .42f),
                F(.25f, .95f, .45f, .8f),
                F(.8f, .05f, .85f, .12f), F(.65f, .42f, .7f, .48f)
            };
            _dots[33] = new[] { F(.55f, .25f, .04f), F(.82f, .12f, .04f), F(.42f, .82f, .04f) };

            // 34: H with hooked ends + center diamond points + dots
            _lines[34] = new[] {
                F(.15f, .1f, .15f, .9f), F(.85f, .1f, .85f, .9f),
                F(.15f, .5f, .85f, .5f),
                F(.15f, .1f, .25f, .05f), F(.15f, .9f, .08f, .95f),
                F(.85f, .1f, .92f, .05f), F(.85f, .9f, .75f, .95f),
                F(.42f, .5f, .5f, .42f, .58f, .5f, .5f, .58f, .42f, .5f)
            };
            _dots[34] = new[] { F(.5f, .5f, .04f) };

            // 35: Angular K + extended arms + serif + dots
            _lines[35] = new[] {
                F(.2f, .05f, .2f, .95f),
                F(.2f, .5f, .72f, .08f), F(.2f, .5f, .72f, .92f),
                F(.72f, .08f, .8f, .05f), F(.72f, .92f, .8f, .95f),
                F(.2f, .05f, .12f, .05f), F(.2f, .95f, .12f, .95f),
                F(.45f, .3f, .52f, .28f)
            };
            _dots[35] = new[] { F(.8f, .05f, .04f), F(.8f, .95f, .04f) };

            // 36: Double-barred cross + angled ends + dots
            _lines[36] = new[] {
                F(.5f, .05f, .5f, .95f),
                F(.15f, .32f, .85f, .32f), F(.15f, .62f, .85f, .62f),
                F(.15f, .32f, .08f, .25f), F(.85f, .32f, .92f, .25f),
                F(.15f, .62f, .08f, .7f), F(.85f, .62f, .92f, .7f),
                F(.5f, .05f, .45f, 0f), F(.5f, .95f, .55f, 1f)
            };
            _dots[36] = new[] { F(.5f, .47f, .04f) };

            // 37: Square spiral + extending arm + dots
            _lines[37] = new[] {
                F(.12f, .12f, .88f, .12f, .88f, .88f, .12f, .88f, .12f, .3f,
                  .7f, .3f, .7f, .7f, .3f, .7f, .3f, .45f, .55f, .45f),
                F(.55f, .45f, .55f, .55f),
                F(.55f, .55f, .62f, .58f)
            };
            _dots[37] = new[] { F(.55f, .55f, .04f), F(.12f, .12f, .04f) };

            // 38: Inverted V + crossed legs + upper spike + accent
            _lines[38] = new[] {
                F(.5f, .12f, .15f, .7f), F(.5f, .12f, .85f, .7f),
                F(.35f, .7f, .72f, .92f), F(.65f, .7f, .28f, .92f),
                F(.5f, .12f, .5f, 0f),
                F(.5f, 0f, .45f, .05f), F(.5f, 0f, .55f, .05f),
                F(.15f, .7f, .08f, .72f), F(.85f, .7f, .92f, .72f)
            };
            _dots[38] = new[] { F(.5f, .42f, .04f) };

            // 39: Angular M + internal ticks + base line + dots
            _lines[39] = new[] {
                F(.1f, .92f, .1f, .08f, .5f, .42f, .9f, .08f, .9f, .92f),
                F(.3f, .25f, .35f, .2f), F(.7f, .25f, .65f, .2f),
                F(.1f, .92f, .9f, .92f),
                F(.5f, .42f, .5f, .55f)
            };
            _dots[39] = new[] { F(.5f, .55f, .04f), F(.3f, .72f, .04f), F(.7f, .72f, .04f) };

            // 40: Vertical + 4 alternating branches + terminal dots
            _lines[40] = new[] {
                F(.5f, .02f, .5f, .98f),
                F(.5f, .18f, .82f, .08f), F(.5f, .38f, .18f, .28f),
                F(.5f, .58f, .82f, .48f), F(.5f, .78f, .18f, .68f),
                F(.82f, .08f, .88f, .05f), F(.18f, .28f, .12f, .25f),
                F(.82f, .48f, .88f, .45f), F(.18f, .68f, .12f, .65f)
            };
            _dots[40] = new[] { F(.88f, .05f, .04f), F(.12f, .25f, .04f), F(.88f, .45f, .04f), F(.12f, .65f, .04f) };

            // 41: Angular P-shape + lower extension + tick + dot
            _lines[41] = new[] {
                F(.2f, .95f, .2f, .05f, .72f, .05f, .72f, .42f, .2f, .42f),
                F(.2f, .95f, .45f, .82f),
                F(.45f, .82f, .5f, .88f),
                F(.72f, .05f, .78f, 0f), F(.72f, .42f, .78f, .48f),
                F(.45f, .22f, .55f, .22f)
            };
            _dots[41] = new[] { F(.48f, .22f, .04f), F(.5f, .88f, .04f) };

            // 42: Angular D-shape + internal dot + external ticks
            _lines[42] = new[] {
                F(.22f, .05f, .22f, .95f),
                F(.22f, .05f, .65f, .05f, .82f, .25f, .82f, .75f, .65f, .95f, .22f, .95f),
                F(.22f, .05f, .15f, .05f), F(.22f, .95f, .15f, .95f),
                F(.82f, .5f, .9f, .5f)
            };
            _dots[42] = new[] { F(.48f, .5f, .05f), F(.9f, .5f, .04f) };

            // 43: Z with serifs + center dot + side accents
            _lines[43] = new[] {
                F(.15f, .1f, .85f, .1f, .15f, .9f, .85f, .9f),
                F(.15f, .1f, .15f, .2f), F(.85f, .1f, .85f, .2f),
                F(.15f, .9f, .15f, .8f), F(.85f, .9f, .85f, .8f),
                F(.42f, .5f, .58f, .5f)
            };
            _dots[43] = new[] { F(.5f, .5f, .05f), F(.08f, .1f, .04f), F(.92f, .9f, .04f) };

            // 44: Arrow rune + bar + base fork + dot
            _lines[44] = new[] {
                F(.5f, 0f, .5f, .72f),
                F(.2f, .25f, .5f, 0f, .8f, .25f),
                F(.25f, .45f, .75f, .45f),
                F(.5f, .72f, .3f, .95f), F(.5f, .72f, .7f, .95f),
                F(.2f, .25f, .15f, .3f), F(.8f, .25f, .85f, .3f)
            };
            _dots[44] = new[] { F(.5f, .45f, .04f), F(.3f, .95f, .04f), F(.7f, .95f, .04f) };

            // 45: Angular L + extended serif + cross strokes + dot
            _lines[45] = new[] {
                F(.25f, .05f, .25f, .85f, .85f, .85f),
                F(.25f, .05f, .35f, 0f),
                F(.85f, .85f, .85f, .75f),
                F(.25f, .35f, .55f, .35f), F(.25f, .6f, .45f, .6f),
                F(.55f, .35f, .6f, .3f), F(.45f, .6f, .5f, .55f)
            };
            _dots[45] = new[] { F(.35f, 0f, .04f), F(.7f, .85f, .04f) };

            // 46: Two interlocked V shapes + connecting bar + dots
            _lines[46] = new[] {
                F(.15f, .15f, .42f, .55f, .15f, .85f),
                F(.85f, .15f, .58f, .55f, .85f, .85f),
                F(.42f, .55f, .58f, .55f),
                F(.15f, .15f, .08f, .1f), F(.85f, .15f, .92f, .1f),
                F(.15f, .85f, .08f, .9f), F(.85f, .85f, .92f, .9f)
            };
            _dots[46] = new[] { F(.5f, .55f, .04f), F(.5f, .25f, .04f), F(.5f, .82f, .04f) };

            // 47: Angular sigma + internal tick + base accent + dot
            _lines[47] = new[] {
                F(.78f, .08f, .22f, .08f, .5f, .5f, .22f, .92f, .78f, .92f),
                F(.5f, .5f, .62f, .5f),
                F(.78f, .08f, .82f, .15f), F(.78f, .92f, .82f, .85f),
                F(.22f, .08f, .18f, .15f), F(.22f, .92f, .18f, .85f)
            };
            _dots[47] = new[] { F(.62f, .5f, .04f), F(.5f, .28f, .04f), F(.5f, .72f, .04f) };

            // 48: Vertical + angular flag + base T + dots
            _lines[48] = new[] {
                F(.3f, .05f, .3f, .95f),
                F(.3f, .05f, .82f, .18f, .82f, .38f, .3f, .45f),
                F(.15f, .95f, .55f, .95f),
                F(.82f, .18f, .88f, .15f), F(.82f, .38f, .88f, .42f)
            };
            _dots[48] = new[] { F(.55f, .28f, .04f), F(.55f, .95f, .04f), F(.15f, .95f, .04f) };

            // 49: Gate rune + internal fork + side hooks + dots
            _lines[49] = new[] {
                F(.15f, .92f, .15f, .08f, .85f, .08f, .85f, .92f),
                F(.5f, .08f, .5f, .55f, .3f, .75f),
                F(.5f, .55f, .7f, .75f),
                F(.15f, .92f, .08f, .88f), F(.85f, .92f, .92f, .88f),
                F(.3f, .75f, .25f, .82f), F(.7f, .75f, .75f, .82f)
            };
            _dots[49] = new[] { F(.5f, .35f, .04f), F(.25f, .82f, .04f), F(.75f, .82f, .04f) };

            // ═══════════════════════════════════════════════════════════════
            //  SCRIPT C (50–74): FLOWING ORGANIC
            //  Curved tendrils, vine-like forms, and sinuous strokes.
            // ═══════════════════════════════════════════════════════════════

            // 50: Flowing S-curve + side branches + tip dots
            _lines[50] = new[] {
                F(.72f, .05f, .6f, .15f, .35f, .3f, .28f, .5f, .35f, .7f, .6f, .85f, .72f, .95f),
                F(.35f, .3f, .18f, .22f), F(.6f, .85f, .78f, .78f),
                F(.28f, .5f, .15f, .52f), F(.6f, .5f, .72f, .48f)
            };
            _dots[50] = new[] { F(.18f, .22f, .04f), F(.78f, .78f, .04f), F(.72f, .05f, .04f), F(.72f, .95f, .04f) };

            // 51: Tendril spiral + side branches + dots
            _lines[51] = new[] {
                F(.5f, .95f, .5f, .6f, .35f, .45f, .32f, .28f, .4f, .15f, .55f, .12f, .65f, .18f, .68f, .32f, .58f, .42f, .48f, .42f),
                F(.35f, .45f, .18f, .4f), F(.55f, .12f, .55f, .02f),
                F(.68f, .32f, .82f, .28f)
            };
            _dots[51] = new[] { F(.48f, .42f, .04f), F(.18f, .4f, .04f), F(.82f, .28f, .04f), F(.55f, .02f, .04f) };

            // 52: Double wave + vertical connectors
            _lines[52] = new[] {
                F(.08f, .2f, .25f, .12f, .42f, .2f, .58f, .12f, .75f, .2f, .92f, .12f),
                F(.08f, .8f, .25f, .72f, .42f, .8f, .58f, .72f, .75f, .8f, .92f, .72f),
                F(.25f, .12f, .25f, .72f), F(.58f, .12f, .58f, .72f),
                F(.42f, .2f, .42f, .8f)
            };
            _dots[52] = new[] { F(.08f, .2f, .04f), F(.92f, .12f, .04f), F(.08f, .8f, .04f), F(.92f, .72f, .04f) };

            // 53: Organic Y + curved branches + root extension + dots
            _lines[53] = new[] {
                F(.12f, .05f, .2f, .15f, .35f, .3f, .45f, .48f),
                F(.88f, .05f, .8f, .15f, .65f, .3f, .55f, .48f),
                F(.45f, .48f, .5f, .5f, .55f, .48f),
                F(.5f, .5f, .48f, .65f, .5f, .78f, .52f, .92f),
                F(.52f, .92f, .45f, .98f), F(.52f, .92f, .58f, .98f)
            };
            _dots[53] = new[] { F(.12f, .05f, .04f), F(.88f, .05f, .04f), F(.45f, .98f, .04f), F(.58f, .98f, .04f) };

            // 54: Curved lambda + inner loop + accent
            _lines[54] = new[] {
                F(.35f, .02f, .4f, .12f, .5f, .35f, .55f, .55f, .52f, .72f, .42f, .85f, .3f, .92f),
                F(.5f, .35f, .65f, .45f, .72f, .6f, .68f, .72f, .58f, .78f),
                F(.55f, .55f, .62f, .55f), F(.3f, .92f, .22f, .95f)
            };
            _dots[54] = new[] { F(.35f, .02f, .04f), F(.22f, .95f, .04f), F(.58f, .78f, .04f) };

            // 55: Flowing J-hook + side tendril + tip dots
            _lines[55] = new[] {
                F(.62f, .02f, .62f, .35f, .58f, .55f, .48f, .7f, .32f, .78f, .22f, .72f, .2f, .58f),
                F(.58f, .55f, .72f, .6f, .82f, .55f),
                F(.62f, .02f, .55f, .02f), F(.2f, .58f, .15f, .52f)
            };
            _dots[55] = new[] { F(.82f, .55f, .04f), F(.15f, .52f, .04f), F(.55f, .02f, .04f) };

            // 56: Organic figure-8 + extending tendrils + dots
            _lines[56] = new[] {
                F(.5f, .5f, .35f, .35f, .22f, .25f, .22f, .15f, .35f, .08f, .5f, .12f, .62f, .22f, .62f, .35f, .5f, .5f,
                  .38f, .62f, .35f, .75f, .42f, .88f, .55f, .92f, .68f, .82f, .68f, .68f, .5f, .5f),
                F(.22f, .15f, .12f, .08f), F(.68f, .82f, .82f, .9f)
            };
            _dots[56] = new[] { F(.12f, .08f, .04f), F(.82f, .9f, .04f), F(.5f, .5f, .04f) };

            // 57: Vine ascending + 3 leaf-branches + dots
            _lines[57] = new[] {
                F(.5f, .95f, .48f, .78f, .45f, .6f, .48f, .42f, .52f, .28f, .5f, .12f, .48f, .02f),
                F(.45f, .6f, .28f, .52f, .18f, .48f),
                F(.48f, .42f, .68f, .35f, .78f, .32f),
                F(.52f, .28f, .35f, .18f, .25f, .15f),
                F(.18f, .48f, .12f, .45f), F(.78f, .32f, .85f, .28f)
            };
            _dots[57] = new[] { F(.12f, .45f, .04f), F(.85f, .28f, .04f), F(.25f, .15f, .04f), F(.48f, .02f, .04f) };

            // 58: Curved trident (3 prongs) + base
            _lines[58] = new[] {
                F(.5f, .95f, .5f, .55f),
                F(.5f, .55f, .42f, .35f, .3f, .18f, .22f, .05f),
                F(.5f, .55f, .5f, .3f, .5f, .05f),
                F(.5f, .55f, .58f, .35f, .7f, .18f, .78f, .05f),
                F(.22f, .05f, .15f, .02f), F(.78f, .05f, .85f, .02f),
                F(.5f, .05f, .5f, 0f)
            };
            _dots[58] = new[] { F(.15f, .02f, .04f), F(.85f, .02f, .04f), F(.5f, 0f, .04f), F(.5f, .95f, .04f) };

            // 59: Organic diamond + internal curve + dot
            _lines[59] = new[] {
                F(.5f, .05f, .85f, .35f, .82f, .55f, .65f, .72f, .5f, .95f,
                  .35f, .72f, .18f, .55f, .15f, .35f, .5f, .05f),
                F(.35f, .35f, .45f, .5f, .55f, .55f, .68f, .48f),
                F(.5f, .05f, .5f, .12f)
            };
            _dots[59] = new[] { F(.5f, .5f, .05f), F(.5f, .12f, .04f) };

            // 60: Double helix + connecting rungs + dots
            _lines[60] = new[] {
                F(.3f, .02f, .38f, .15f, .55f, .3f, .62f, .5f, .55f, .7f, .38f, .85f, .3f, .98f),
                F(.7f, .02f, .62f, .15f, .45f, .3f, .38f, .5f, .45f, .7f, .62f, .85f, .7f, .98f),
                F(.42f, .22f, .58f, .22f),
                F(.58f, .42f, .42f, .42f),
                F(.42f, .62f, .58f, .62f),
                F(.58f, .8f, .42f, .8f)
            };
            _dots[60] = new[] { F(.3f, .02f, .04f), F(.7f, .02f, .04f), F(.3f, .98f, .04f), F(.7f, .98f, .04f) };

            // 61: Flowing curved T + hook endings + accent dots
            _lines[61] = new[] {
                F(.08f, .18f, .2f, .12f, .4f, .1f, .6f, .1f, .8f, .12f, .92f, .18f),
                F(.5f, .1f, .48f, .32f, .5f, .55f, .52f, .72f, .48f, .88f),
                F(.48f, .88f, .38f, .92f), F(.92f, .18f, .95f, .25f),
                F(.08f, .18f, .05f, .25f)
            };
            _dots[61] = new[] { F(.38f, .92f, .04f), F(.95f, .25f, .04f), F(.05f, .25f, .04f), F(.5f, .55f, .04f) };

            // 62: Sinuous ascending path + branching top + dots
            _lines[62] = new[] {
                F(.45f, .95f, .42f, .8f, .48f, .65f, .55f, .5f, .52f, .35f, .48f, .2f),
                F(.48f, .2f, .3f, .08f, .18f, .05f),
                F(.48f, .2f, .65f, .08f, .78f, .02f),
                F(.55f, .5f, .68f, .45f), F(.42f, .8f, .3f, .82f)
            };
            _dots[62] = new[] { F(.18f, .05f, .04f), F(.78f, .02f, .04f), F(.68f, .45f, .04f), F(.3f, .82f, .04f) };

            // 63: Organic circle + 3 tendril extensions + dots
            _circles[63] = new[] { F(.5f, .48f, .2f) };
            _lines[63] = new[] {
                F(.5f, .28f, .48f, .15f, .42f, .05f, .35f, 0f),
                F(.32f, .55f, .2f, .62f, .1f, .72f, .05f, .82f),
                F(.68f, .55f, .8f, .62f, .9f, .72f, .95f, .82f),
                F(.35f, 0f, .28f, .02f), F(.05f, .82f, .02f, .88f), F(.95f, .82f, .98f, .88f)
            };
            _dots[63] = new[] { F(.28f, .02f, .04f), F(.02f, .88f, .04f), F(.98f, .88f, .04f), F(.5f, .48f, .04f) };

            // 64: Curved V + internal crossbar + flowing tail + dot
            _lines[64] = new[] {
                F(.12f, .05f, .22f, .18f, .38f, .38f, .5f, .55f),
                F(.88f, .05f, .78f, .18f, .62f, .38f, .5f, .55f),
                F(.32f, .3f, .68f, .3f),
                F(.5f, .55f, .48f, .7f, .42f, .82f, .38f, .95f),
                F(.38f, .95f, .32f, .98f)
            };
            _dots[64] = new[] { F(.12f, .05f, .04f), F(.88f, .05f, .04f), F(.32f, .98f, .04f), F(.5f, .3f, .04f) };

            // 65: Wave + vertical through nodes + dots
            _lines[65] = new[] {
                F(.05f, .38f, .22f, .25f, .42f, .38f, .58f, .25f, .78f, .38f, .95f, .25f),
                F(.22f, .25f, .22f, .82f), F(.58f, .25f, .58f, .82f),
                F(.22f, .82f, .18f, .88f), F(.58f, .82f, .62f, .88f),
                F(.42f, .55f, .22f, .55f), F(.78f, .55f, .58f, .55f)
            };
            _dots[65] = new[] { F(.18f, .88f, .04f), F(.62f, .88f, .04f), F(.42f, .38f, .04f), F(.78f, .38f, .04f) };

            // 66: Organic arch + pendant + base curves + dots
            _lines[66] = new[] {
                F(.1f, .72f, .15f, .42f, .25f, .22f, .42f, .1f, .58f, .1f, .75f, .22f, .85f, .42f, .9f, .72f),
                F(.5f, .1f, .5f, .38f, .48f, .52f, .5f, .65f),
                F(.5f, .65f, .42f, .72f), F(.5f, .65f, .58f, .72f),
                F(.1f, .72f, .08f, .82f, .15f, .88f),
                F(.9f, .72f, .92f, .82f, .85f, .88f)
            };
            _dots[66] = new[] { F(.15f, .88f, .04f), F(.85f, .88f, .04f), F(.5f, .38f, .04f) };

            // 67: Flowing K with curved arms + accent dots
            _lines[67] = new[] {
                F(.25f, .02f, .28f, .18f, .3f, .38f, .28f, .58f, .25f, .78f, .22f, .95f),
                F(.3f, .38f, .42f, .3f, .58f, .18f, .72f, .1f, .82f, .05f),
                F(.28f, .58f, .42f, .68f, .58f, .78f, .72f, .85f, .82f, .92f),
                F(.82f, .05f, .88f, .02f), F(.82f, .92f, .88f, .95f)
            };
            _dots[67] = new[] { F(.88f, .02f, .04f), F(.88f, .95f, .04f), F(.22f, .95f, .04f), F(.25f, .02f, .04f) };

            // 68: Tendril spiral + side hook + dots
            _lines[68] = new[] {
                F(.78f, .88f, .65f, .82f, .45f, .7f, .35f, .55f, .38f, .38f, .5f, .25f, .62f, .22f, .68f, .3f, .62f, .42f, .52f, .45f),
                F(.35f, .55f, .2f, .58f, .12f, .52f, .12f, .42f, .18f, .35f),
                F(.78f, .88f, .85f, .92f)
            };
            _dots[68] = new[] { F(.52f, .45f, .04f), F(.18f, .35f, .04f), F(.85f, .92f, .04f) };

            // 69: Flowing omega + tendrils + dots
            _lines[69] = new[] {
                F(.05f, .82f, .12f, .6f, .22f, .38f, .38f, .22f, .5f, .2f, .62f, .22f, .78f, .38f, .88f, .6f, .95f, .82f),
                F(.05f, .82f, .02f, .92f), F(.95f, .82f, .98f, .92f),
                F(.5f, .2f, .5f, .08f), F(.5f, .08f, .45f, .02f), F(.5f, .08f, .55f, .02f)
            };
            _dots[69] = new[] { F(.02f, .92f, .04f), F(.98f, .92f, .04f), F(.45f, .02f, .04f), F(.55f, .02f, .04f), F(.5f, .55f, .04f) };

            // 70: Curved cross + flowing arms + center dot
            _lines[70] = new[] {
                F(.5f, .02f, .48f, .2f, .5f, .38f, .5f, .62f, .52f, .8f, .5f, .98f),
                F(.02f, .5f, .2f, .48f, .38f, .5f, .62f, .5f, .8f, .52f, .98f, .5f),
                F(.5f, .02f, .45f, 0f), F(.5f, .98f, .55f, 1f),
                F(.02f, .5f, 0f, .45f), F(.98f, .5f, 1f, .55f)
            };
            _dots[70] = new[] { F(.5f, .5f, .05f) };

            // 71: Organic chevron + inner curve + dots
            _lines[71] = new[] {
                F(.08f, .72f, .22f, .55f, .38f, .35f, .5f, .18f, .62f, .35f, .78f, .55f, .92f, .72f),
                F(.38f, .35f, .42f, .48f, .5f, .55f, .58f, .48f, .62f, .35f),
                F(.08f, .72f, .05f, .78f), F(.92f, .72f, .95f, .78f),
                F(.5f, .18f, .5f, .08f)
            };
            _dots[71] = new[] { F(.05f, .78f, .04f), F(.95f, .78f, .04f), F(.5f, .08f, .04f), F(.5f, .55f, .04f) };

            // 72: Flowing anchor + scrollwork + dots
            _lines[72] = new[] {
                F(.5f, .02f, .5f, .72f),
                F(.5f, .72f, .32f, .82f, .22f, .78f, .2f, .65f, .25f, .55f),
                F(.5f, .72f, .68f, .82f, .78f, .78f, .8f, .65f, .75f, .55f),
                F(.35f, .02f, .65f, .02f),
                F(.5f, .35f, .62f, .3f), F(.5f, .35f, .38f, .3f)
            };
            _dots[72] = new[] { F(.25f, .55f, .04f), F(.75f, .55f, .04f), F(.62f, .3f, .04f), F(.38f, .3f, .04f) };

            // 73: Double organic curve + internal details + dots
            _lines[73] = new[] {
                F(.15f, .05f, .25f, .25f, .42f, .42f, .5f, .55f, .42f, .68f, .25f, .82f, .15f, .95f),
                F(.85f, .05f, .75f, .25f, .58f, .42f, .55f, .58f, .68f, .75f, .82f, .85f, .95f),
                F(.3f, .32f, .42f, .42f), F(.7f, .32f, .58f, .42f),
                F(.3f, .72f, .42f, .68f), F(.7f, .72f, .58f, .68f)
            };
            _dots[73] = new[] { F(.5f, .55f, .05f), F(.15f, .05f, .04f), F(.85f, .05f, .04f), F(.15f, .95f, .04f), F(.85f, .95f, .04f) };

            // 74: Organic starburst (curved 6-arms) + center circle + dots
            _circles[74] = new[] { F(.5f, .5f, .1f) };
            _lines[74] = new[] {
                F(.5f, .4f, .48f, .2f, .42f, .05f),
                F(.5f, .6f, .52f, .8f, .58f, .95f),
                F(.4f, .48f, .22f, .42f, .08f, .35f),
                F(.6f, .52f, .78f, .58f, .92f, .65f),
                F(.42f, .55f, .25f, .72f, .12f, .82f),
                F(.58f, .45f, .75f, .28f, .88f, .18f)
            };
            _dots[74] = new[] { F(.42f, .05f, .04f), F(.58f, .95f, .04f), F(.08f, .35f, .04f), F(.92f, .65f, .04f), F(.12f, .82f, .04f), F(.88f, .18f, .04f) };

            // ═══════════════════════════════════════════════════════════════
            //  SCRIPT D (75–99): TECHNICAL GEOMETRIC
            //  Circuit-trace patterns, nested frames, precise geometry.
            // ═══════════════════════════════════════════════════════════════

            // 75: Circuit right-angle trace + junction dots + via holes
            _lines[75] = new[] {
                F(.1f, .15f, .35f, .15f, .35f, .4f, .65f, .4f, .65f, .65f, .9f, .65f),
                F(.35f, .4f, .35f, .65f, .5f, .65f, .5f, .85f),
                F(.65f, .4f, .65f, .15f, .85f, .15f),
                F(.1f, .15f, .1f, .22f), F(.9f, .65f, .9f, .72f)
            };
            _dots[75] = new[] { F(.35f, .4f, .04f), F(.65f, .4f, .04f), F(.65f, .65f, .04f), F(.5f, .85f, .04f), F(.85f, .15f, .04f) };

            // 76: Chip outline + pin lines + internal grid
            _lines[76] = new[] {
                F(.2f, .15f, .8f, .15f, .8f, .85f, .2f, .85f, .2f, .15f),
                F(.35f, .15f, .35f, .05f), F(.5f, .15f, .5f, .05f), F(.65f, .15f, .65f, .05f),
                F(.35f, .85f, .35f, .95f), F(.5f, .85f, .5f, .95f), F(.65f, .85f, .65f, .95f),
                F(.2f, .4f, .8f, .4f), F(.2f, .6f, .8f, .6f),
                F(.5f, .15f, .5f, .85f)
            };
            _dots[76] = new[] { F(.35f, .4f, .03f), F(.65f, .4f, .03f), F(.35f, .6f, .03f), F(.65f, .6f, .03f) };

            // 77: 3 parallel bus traces + branches + dots
            _lines[77] = new[] {
                F(.15f, .2f, .85f, .2f), F(.15f, .5f, .85f, .5f), F(.15f, .8f, .85f, .8f),
                F(.35f, .2f, .35f, .5f), F(.65f, .5f, .65f, .8f),
                F(.5f, .2f, .5f, .08f), F(.5f, .8f, .5f, .92f),
                F(.15f, .2f, .08f, .2f), F(.85f, .8f, .92f, .8f)
            };
            _dots[77] = new[] { F(.35f, .2f, .04f), F(.35f, .5f, .04f), F(.65f, .5f, .04f), F(.65f, .8f, .04f), F(.5f, .08f, .04f), F(.5f, .92f, .04f) };

            // 78: Angular meander + endpoint dots
            _lines[78] = new[] {
                F(.1f, .1f, .45f, .1f, .45f, .35f, .15f, .35f, .15f, .6f, .55f, .6f, .55f, .85f, .9f, .85f),
                F(.9f, .85f, .9f, .6f, .7f, .6f, .7f, .35f, .9f, .35f, .9f, .1f),
                F(.1f, .1f, .05f, .05f), F(.9f, .1f, .95f, .05f)
            };
            _dots[78] = new[] { F(.05f, .05f, .04f), F(.95f, .05f, .04f), F(.9f, .85f, .04f) };

            // 79: T-junction circuit + loop + branch + dots
            _lines[79] = new[] {
                F(.1f, .35f, .9f, .35f),
                F(.5f, .35f, .5f, .75f),
                F(.5f, .75f, .35f, .75f, .35f, .55f, .65f, .55f, .65f, .75f, .5f, .75f),
                F(.25f, .35f, .25f, .2f), F(.75f, .35f, .75f, .2f),
                F(.5f, .75f, .5f, .9f)
            };
            _dots[79] = new[] { F(.25f, .35f, .04f), F(.75f, .35f, .04f), F(.5f, .35f, .04f), F(.5f, .75f, .04f), F(.25f, .2f, .04f), F(.75f, .2f, .04f), F(.5f, .9f, .04f) };

            // 80: Grid + diagonal fill + corner accents
            _lines[80] = new[] {
                F(.15f, .15f, .85f, .15f, .85f, .85f, .15f, .85f, .15f, .15f),
                F(.5f, .15f, .5f, .85f), F(.15f, .5f, .85f, .5f),
                F(.15f, .15f, .5f, .5f), F(.5f, .5f, .85f, .85f),
                F(.08f, .08f, .15f, .15f), F(.92f, .08f, .85f, .15f),
                F(.08f, .92f, .15f, .85f), F(.92f, .92f, .85f, .85f)
            };
            _dots[80] = new[] { F(.08f, .08f, .04f), F(.92f, .08f, .04f), F(.08f, .92f, .04f), F(.92f, .92f, .04f) };

            // 81: Nested squares (3) + corner connectors
            _lines[81] = new[] {
                F(.05f, .05f, .95f, .05f, .95f, .95f, .05f, .95f, .05f, .05f),
                F(.2f, .2f, .8f, .2f, .8f, .8f, .2f, .8f, .2f, .2f),
                F(.38f, .38f, .62f, .38f, .62f, .62f, .38f, .62f, .38f, .38f),
                F(.05f, .05f, .2f, .2f), F(.95f, .05f, .8f, .2f),
                F(.05f, .95f, .2f, .8f), F(.95f, .95f, .8f, .8f)
            };
            _dots[81] = new[] { F(.5f, .5f, .04f) };

            // 82: Diamond in square + radials + dots
            _lines[82] = new[] {
                F(.1f, .1f, .9f, .1f, .9f, .9f, .1f, .9f, .1f, .1f),
                F(.5f, .2f, .8f, .5f, .5f, .8f, .2f, .5f, .5f, .2f),
                F(.5f, .1f, .5f, .2f), F(.5f, .8f, .5f, .9f),
                F(.1f, .5f, .2f, .5f), F(.8f, .5f, .9f, .5f)
            };
            _dots[82] = new[] { F(.5f, .5f, .05f), F(.5f, .1f, .03f), F(.5f, .9f, .03f), F(.1f, .5f, .03f), F(.9f, .5f, .03f) };

            // 83: Hexagonal frame + internal tri-grid + dots
            _lines[83] = new[] {
                F(.5f, .05f, .88f, .28f, .88f, .72f, .5f, .95f, .12f, .72f, .12f, .28f, .5f, .05f),
                F(.5f, .05f, .5f, .95f),
                F(.12f, .28f, .88f, .72f), F(.88f, .28f, .12f, .72f),
                F(.31f, .17f, .31f, .83f), F(.69f, .17f, .69f, .83f)
            };
            _dots[83] = new[] { F(.5f, .5f, .04f), F(.5f, .05f, .04f), F(.5f, .95f, .04f) };

            // 84: Octagon + internal star + center dot
            _lines[84] = new[] {
                F(.35f, .05f, .65f, .05f, .95f, .35f, .95f, .65f, .65f, .95f, .35f, .95f, .05f, .65f, .05f, .35f, .35f, .05f),
                F(.5f, .15f, .82f, .5f, .5f, .85f, .18f, .5f, .5f, .15f),
                F(.5f, .15f, .18f, .5f), F(.82f, .5f, .5f, .85f)
            };
            _dots[84] = new[] { F(.5f, .5f, .05f) };

            // 85: Cross-bus + 4 junction dots + center diamond
            _lines[85] = new[] {
                F(.5f, .02f, .5f, .98f), F(.02f, .5f, .98f, .5f),
                F(.42f, .42f, .5f, .35f, .58f, .42f, .5f, .5f, .42f, .42f),
                F(.5f, .5f, .5f, .58f),
                F(.25f, .5f, .25f, .38f), F(.75f, .5f, .75f, .62f),
                F(.5f, .25f, .38f, .25f), F(.5f, .75f, .62f, .75f)
            };
            _dots[85] = new[] { F(.25f, .38f, .04f), F(.75f, .62f, .04f), F(.38f, .25f, .04f), F(.62f, .75f, .04f) };

            // 86: Angular circuit loop + branch + terminal dots
            _lines[86] = new[] {
                F(.15f, .15f, .65f, .15f, .65f, .42f, .35f, .42f, .35f, .65f, .65f, .65f, .65f, .85f, .15f, .85f, .15f, .15f),
                F(.65f, .42f, .85f, .42f), F(.85f, .42f, .85f, .55f),
                F(.35f, .65f, .35f, .85f)
            };
            _dots[86] = new[] { F(.15f, .15f, .04f), F(.65f, .15f, .04f), F(.85f, .55f, .04f), F(.35f, .85f, .04f), F(.15f, .85f, .04f) };

            // 87: Multi-level trace + steps + junction dots
            _lines[87] = new[] {
                F(.08f, .82f, .08f, .5f, .32f, .5f, .32f, .25f, .58f, .25f, .58f, .5f, .82f, .5f, .82f, .18f, .92f, .18f),
                F(.32f, .25f, .32f, .12f), F(.58f, .5f, .58f, .62f, .72f, .62f),
                F(.45f, .25f, .45f, .5f)
            };
            _dots[87] = new[] { F(.08f, .82f, .04f), F(.92f, .18f, .04f), F(.32f, .12f, .04f), F(.72f, .62f, .04f), F(.32f, .5f, .04f), F(.58f, .25f, .04f) };

            // 88: Angular antenna + ground plane + signal dots
            _lines[88] = new[] {
                F(.5f, .42f, .5f, .92f),
                F(.5f, .42f, .25f, .12f), F(.5f, .42f, .75f, .12f),
                F(.25f, .12f, .12f, .05f), F(.75f, .12f, .88f, .05f),
                F(.25f, .12f, .25f, .05f), F(.75f, .12f, .75f, .05f),
                F(.3f, .92f, .7f, .92f),
                F(.2f, .85f, .8f, .85f)
            };
            _dots[88] = new[] { F(.12f, .05f, .04f), F(.88f, .05f, .04f), F(.25f, .05f, .04f), F(.75f, .05f, .04f), F(.5f, .92f, .04f) };

            // 89: Power symbol variant + circuit frame + dots
            _lines[89] = new[] {
                F(.5f, .08f, .5f, .38f),
                F(.3f, .22f, .2f, .38f, .18f, .58f, .25f, .72f, .42f, .82f, .58f, .82f, .75f, .72f, .82f, .58f, .8f, .38f, .7f, .22f),
                F(.2f, .82f, .8f, .82f),
                F(.2f, .82f, .2f, .92f), F(.8f, .82f, .8f, .92f),
                F(.5f, .82f, .5f, .92f)
            };
            _dots[89] = new[] { F(.2f, .92f, .04f), F(.8f, .92f, .04f), F(.5f, .92f, .04f) };

            // 90: Trace with via holes + angular path
            _lines[90] = new[] {
                F(.1f, .18f, .3f, .18f, .3f, .42f, .55f, .42f, .55f, .18f, .78f, .18f),
                F(.78f, .18f, .78f, .55f, .55f, .55f, .55f, .78f, .3f, .78f, .3f, .55f, .1f, .55f),
                F(.1f, .55f, .1f, .78f)
            };
            _dots[90] = new[] { F(.3f, .18f, .04f), F(.55f, .42f, .04f), F(.78f, .18f, .04f), F(.55f, .55f, .04f), F(.3f, .78f, .04f), F(.1f, .78f, .04f) };

            // 91: Double-layered angular frame + internal circuit
            _lines[91] = new[] {
                F(.08f, .08f, .92f, .08f, .92f, .92f, .08f, .92f, .08f, .08f),
                F(.22f, .22f, .78f, .22f, .78f, .78f, .22f, .78f, .22f, .22f),
                F(.22f, .35f, .42f, .35f, .42f, .55f, .62f, .55f, .62f, .35f, .78f, .35f),
                F(.42f, .55f, .42f, .78f), F(.62f, .55f, .62f, .78f)
            };
            _dots[91] = new[] { F(.42f, .35f, .03f), F(.62f, .35f, .03f), F(.42f, .55f, .03f), F(.62f, .55f, .03f) };

            // 92: Angular spiral trace + connecting straight + dot
            _lines[92] = new[] {
                F(.88f, .12f, .12f, .12f, .12f, .88f, .75f, .88f, .75f, .28f, .28f, .28f, .28f, .72f, .62f, .72f, .62f, .42f, .42f, .42f, .42f, .58f, .52f, .58f),
                F(.52f, .58f, .52f, .5f),
                F(.88f, .12f, .92f, .08f)
            };
            _dots[92] = new[] { F(.52f, .5f, .04f), F(.92f, .08f, .04f) };

            // 93: Parallel traces + crossover + junction marks
            _lines[93] = new[] {
                F(.08f, .25f, .38f, .25f, .62f, .42f, .92f, .42f),
                F(.08f, .42f, .38f, .42f, .62f, .25f, .92f, .25f),
                F(.08f, .62f, .45f, .62f, .45f, .78f, .92f, .78f),
                F(.08f, .78f, .45f, .78f, .45f, .62f),
                F(.5f, .34f, .5f, .18f)
            };
            _dots[93] = new[] { F(.5f, .34f, .04f), F(.08f, .25f, .03f), F(.92f, .42f, .03f), F(.08f, .42f, .03f), F(.92f, .25f, .03f), F(.5f, .18f, .04f) };

            // 94: Complex 4-way junction + routing + dots
            _lines[94] = new[] {
                F(.5f, .05f, .5f, .95f), F(.05f, .5f, .95f, .5f),
                F(.5f, .3f, .3f, .3f, .3f, .5f),
                F(.5f, .7f, .7f, .7f, .7f, .5f),
                F(.3f, .3f, .15f, .15f), F(.7f, .7f, .85f, .85f),
                F(.3f, .5f, .15f, .5f), F(.7f, .5f, .85f, .5f)
            };
            _dots[94] = new[] { F(.5f, .5f, .05f), F(.15f, .15f, .04f), F(.85f, .85f, .04f), F(.15f, .5f, .04f), F(.85f, .5f, .04f) };

            // 95: Angular frame + selective grid + corner dots
            _lines[95] = new[] {
                F(.1f, .1f, .9f, .1f, .9f, .9f, .1f, .9f, .1f, .1f),
                F(.1f, .38f, .9f, .38f), F(.1f, .62f, .9f, .62f),
                F(.38f, .1f, .38f, .38f), F(.62f, .62f, .62f, .9f),
                F(.38f, .62f, .38f, .9f), F(.62f, .1f, .62f, .38f),
                F(.38f, .38f, .62f, .62f)
            };
            _dots[95] = new[] { F(.1f, .1f, .04f), F(.9f, .1f, .04f), F(.1f, .9f, .04f), F(.9f, .9f, .04f), F(.5f, .5f, .04f) };

            // 96: Stepped pyramid + accent dots
            _lines[96] = new[] {
                F(.5f, .08f, .5f, .22f),
                F(.3f, .22f, .7f, .22f), F(.3f, .22f, .3f, .42f), F(.7f, .22f, .7f, .42f),
                F(.18f, .42f, .82f, .42f), F(.18f, .42f, .18f, .62f), F(.82f, .42f, .82f, .62f),
                F(.08f, .62f, .92f, .62f), F(.08f, .62f, .08f, .82f), F(.92f, .62f, .92f, .82f),
                F(.08f, .82f, .92f, .82f)
            };
            _dots[96] = new[] { F(.5f, .08f, .04f), F(.5f, .52f, .04f), F(.5f, .72f, .04f) };

            // 97: Circuit board corner trace + via dots + accent
            _lines[97] = new[] {
                F(.08f, .08f, .55f, .08f, .55f, .35f, .82f, .35f, .82f, .62f, .55f, .62f, .55f, .88f, .08f, .88f),
                F(.08f, .88f, .08f, .55f), F(.08f, .08f, .08f, .35f),
                F(.82f, .35f, .92f, .28f), F(.82f, .62f, .92f, .68f)
            };
            _dots[97] = new[] { F(.55f, .08f, .04f), F(.55f, .35f, .04f), F(.82f, .35f, .04f), F(.82f, .62f, .04f), F(.55f, .62f, .04f), F(.55f, .88f, .04f), F(.08f, .55f, .04f) };

            // 98: Binary tree (3 levels) + terminal dots
            _lines[98] = new[] {
                F(.5f, .88f, .5f, .62f),
                F(.5f, .62f, .25f, .42f), F(.5f, .62f, .75f, .42f),
                F(.25f, .42f, .12f, .25f), F(.25f, .42f, .38f, .25f),
                F(.75f, .42f, .62f, .25f), F(.75f, .42f, .88f, .25f),
                F(.12f, .25f, .06f, .15f), F(.12f, .25f, .18f, .15f),
                F(.38f, .25f, .32f, .15f), F(.38f, .25f, .44f, .15f),
                F(.62f, .25f, .56f, .15f), F(.62f, .25f, .68f, .15f),
                F(.88f, .25f, .82f, .15f), F(.88f, .25f, .94f, .15f)
            };
            _dots[98] = new[] { F(.06f, .15f, .03f), F(.18f, .15f, .03f), F(.32f, .15f, .03f), F(.44f, .15f, .03f),
                F(.56f, .15f, .03f), F(.68f, .15f, .03f), F(.82f, .15f, .03f), F(.94f, .15f, .03f), F(.5f, .88f, .04f) };

            // 99: Alien entity — elongated head, crown spikes, 3 asymmetric
            //     eyes, angular mandibles, sensor tendrils, chin tentacle
            _circles[99] = new[] { F(.5f, .42f, .3f), F(.5f, .32f, .18f) };
            _lines[99] = new[] {
                // crown spikes
                F(.32f, .15f, .25f, 0f), F(.42f, .14f, .4f, .02f), F(.58f, .14f, .6f, .02f), F(.68f, .15f, .75f, 0f),
                // left mandible with joint
                F(.28f, .6f, .2f, .72f, .15f, .82f, .22f, .9f, .35f, .88f),
                // right mandible with joint
                F(.72f, .6f, .8f, .72f, .85f, .82f, .78f, .9f, .65f, .88f),
                // chin tentacle
                F(.5f, .72f, .48f, .82f, .5f, .92f, .52f, .98f),
                // left sensor tendril
                F(.22f, .38f, .1f, .32f, .05f, .25f),
                // right sensor tendril
                F(.78f, .38f, .9f, .32f, .95f, .25f),
                // brow ridge
                F(.3f, .25f, .5f, .22f, .7f, .25f)
            };
            _dots[99] = new[] {
                F(.38f, .35f, .05f),   // left eye
                F(.62f, .35f, .05f),   // right eye
                F(.5f, .46f, .04f),    // third eye (lower center)
                F(.05f, .25f, .03f),   // left sensor tip
                F(.95f, .25f, .03f),   // right sensor tip
                F(.52f, .98f, .03f)    // chin tip
            };
        }
    }
}