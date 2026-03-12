using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BlockDrop
{
    /// <summary>
    /// 50 procedurally-defined alien symbol glyphs stored as normalised 0–1
    /// vector data. Each glyph is composed of polylines, circle outlines,
    /// and filled dots that scale to any cell size and render in any colour.
    /// </summary>
    internal static class AlienGlyphs
    {
        public const int Count = 50;

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
            // ── 0–4: Basic crosses & diagonals ─────────────────────────
            // 0: Cross (+)
            _lines[0] = new[] { F(.5f, 0f, .5f, 1f), F(0f, .5f, 1f, .5f) };

            // 1: Saltire (X)
            _lines[1] = new[] { F(0f, 0f, 1f, 1f), F(1f, 0f, 0f, 1f) };

            // 2: Asterisk — 6 spokes
            _lines[2] = new[] { F(.5f, 0f, .5f, 1f), F(.07f, .25f, .93f, .75f), F(.93f, .25f, .07f, .75f) };

            // 3: Starburst — 8 spokes from centre
            _lines[3] = new[] {
                F(.5f, 0f, .5f, 1f), F(0f, .5f, 1f, .5f),
                F(.15f, .15f, .85f, .85f), F(.85f, .15f, .15f, .85f)
            };

            // 4: Hash / grid (#)
            _lines[4] = new[] {
                F(.35f, 0f, .35f, 1f), F(.65f, 0f, .65f, 1f),
                F(0f, .35f, 1f, .35f), F(0f, .65f, 1f, .65f)
            };

            // ── 5–9: Circles & circle combos ──────────────────────────
            // 5: Circle
            _circles[5] = new[] { F(.5f, .5f, .4f) };

            // 6: Circle with centre dot ⊙
            _circles[6] = new[] { F(.5f, .5f, .4f) };
            _dots[6] = new[] { F(.5f, .5f, .09f) };

            // 7: Circle with cross ⊕
            _circles[7] = new[] { F(.5f, .5f, .4f) };
            _lines[7] = new[] { F(.5f, .1f, .5f, .9f), F(.1f, .5f, .9f, .5f) };

            // 8: Circle with diagonal ⊘
            _circles[8] = new[] { F(.5f, .5f, .4f) };
            _lines[8] = new[] { F(.22f, .22f, .78f, .78f) };

            // 9: Circle with horizontal bar
            _circles[9] = new[] { F(.5f, .5f, .4f) };
            _lines[9] = new[] { F(.1f, .5f, .9f, .5f) };

            // ── 10–14: Triangles & polygons ───────────────────────────
            // 10: Diamond ◇
            _lines[10] = new[] { F(.5f, 0f, 1f, .5f, .5f, 1f, 0f, .5f, .5f, 0f) };

            // 11: Triangle up △
            _lines[11] = new[] { F(.5f, 0f, 1f, 1f, 0f, 1f, .5f, 0f) };

            // 12: Triangle down ▽
            _lines[12] = new[] { F(0f, 0f, 1f, 0f, .5f, 1f, 0f, 0f) };

            // 13: Pentagon
            _lines[13] = new[] { F(.50f, .05f, .93f, .36f, .76f, .86f, .24f, .86f, .07f, .36f, .50f, .05f) };

            // 14: Hexagon
            _lines[14] = new[] { F(.50f, .05f, .89f, .28f, .89f, .72f, .50f, .95f, .11f, .72f, .11f, .28f, .50f, .05f) };

            // ── 15–19: Arrows & chevrons ──────────────────────────────
            // 15: Arrow up ↑
            _lines[15] = new[] { F(.5f, 0f, .5f, 1f), F(.2f, .3f, .5f, 0f, .8f, .3f) };

            // 16: Arrow down ↓
            _lines[16] = new[] { F(.5f, 0f, .5f, 1f), F(.2f, .7f, .5f, 1f, .8f, .7f) };

            // 17: Chevron up ∧
            _lines[17] = new[] { F(.1f, .7f, .5f, .2f, .9f, .7f) };

            // 18: Double chevron ↟
            _lines[18] = new[] { F(.15f, .5f, .5f, .15f, .85f, .5f), F(.15f, .85f, .5f, .5f, .85f, .85f) };

            // 19: Chevron down ∨
            _lines[19] = new[] { F(.1f, .3f, .5f, .8f, .9f, .3f) };

            // ── 20–24: Letter-like runes ──────────────────────────────
            // 20: T-shape ⊤
            _lines[20] = new[] { F(0f, 0f, 1f, 0f), F(.5f, 0f, .5f, 1f) };

            // 21: Inverted T ⊥
            _lines[21] = new[] { F(0f, 1f, 1f, 1f), F(.5f, 0f, .5f, 1f) };

            // 22: Y-shape
            _lines[22] = new[] { F(0f, 0f, .5f, .5f, 1f, 0f), F(.5f, .5f, .5f, 1f) };

            // 23: Inverted Y
            _lines[23] = new[] { F(0f, 1f, .5f, .5f, 1f, 1f), F(.5f, .5f, .5f, 0f) };

            // 24: H-shape
            _lines[24] = new[] { F(0f, 0f, 0f, 1f), F(1f, 0f, 1f, 1f), F(0f, .5f, 1f, .5f) };

            // ── 25–29: Structural runes ───────────────────────────────
            // 25: I-beam
            _lines[25] = new[] { F(.15f, 0f, .85f, 0f), F(.15f, 1f, .85f, 1f), F(.5f, 0f, .5f, 1f) };

            // 26: Gate / Pi (Π)
            _lines[26] = new[] { F(.15f, 1f, .15f, 0f, .85f, 0f, .85f, 1f) };

            // 27: Psi / Trident (Ψ)
            _lines[27] = new[] {
                F(.5f, .5f, .5f, 1f),
                F(.1f, 0f, .1f, .5f), F(.9f, 0f, .9f, .5f),
                F(.1f, .5f, .9f, .5f)
            };

            // 28: Omega (Ω) approximation
            _lines[28] = new[] { F(0f, 1f, .2f, .65f, .15f, .3f, .35f, .05f, .65f, .05f, .85f, .3f, .8f, .65f, 1f, 1f) };

            // 29: Angular S
            _lines[29] = new[] { F(.85f, 0f, .15f, 0f, .15f, .5f, .85f, .5f, .85f, 1f, .15f, 1f) };

            // ── 30–34: Pattern runes ──────────────────────────────────
            // 30: Hourglass ⌛
            _lines[30] = new[] { F(0f, 0f, 1f, 0f, .5f, .5f, 1f, 1f, 0f, 1f, .5f, .5f, 0f, 0f) };

            // 31: Bowtie ⋈
            _lines[31] = new[] { F(0f, 0f, .5f, .5f, 0f, 1f, 0f, 0f), F(1f, 0f, .5f, .5f, 1f, 1f, 1f, 0f) };

            // 32: Lightning bolt ⚡
            _lines[32] = new[] { F(.65f, 0f, .3f, .42f, .7f, .42f, .35f, 1f) };

            // 33: Zigzag
            _lines[33] = new[] { F(.25f, 0f, .75f, .25f, .25f, .5f, .75f, .75f, .25f, 1f) };

            // 34: Square spiral
            _lines[34] = new[] { F(0f, 0f, 1f, 0f, 1f, 1f, 0f, 1f, 0f, .25f, .75f, .25f, .75f, .75f, .25f, .75f, .25f, .45f, .55f, .45f) };

            // ── 35–39: Dot patterns ───────────────────────────────────
            // 35: Three dots triangle (∴)
            _dots[35] = new[] { F(.5f, .18f, .1f), F(.2f, .82f, .1f), F(.8f, .82f, .1f) };

            // 36: Three dots vertical
            _dots[36] = new[] { F(.5f, .12f, .1f), F(.5f, .5f, .1f), F(.5f, .88f, .1f) };

            // 37: Four dots square (∷)
            _dots[37] = new[] { F(.25f, .25f, .1f), F(.75f, .25f, .1f), F(.25f, .75f, .1f), F(.75f, .75f, .1f) };

            // 38: Quincunx — five dots (⁙)
            _dots[38] = new[] { F(.2f, .2f, .08f), F(.8f, .2f, .08f), F(.5f, .5f, .08f), F(.2f, .8f, .08f), F(.8f, .8f, .08f) };

            // 39: Six dots (2 × 3)
            _dots[39] = new[] {
                F(.3f, .18f, .08f), F(.7f, .18f, .08f),
                F(.3f, .5f, .08f), F(.7f, .5f, .08f),
                F(.3f, .82f, .08f), F(.7f, .82f, .08f)
            };

            // ── 40–44: Compound symbols ───────────────────────────────
            // 40: Crosshair ⌖
            _circles[40] = new[] { F(.5f, .5f, .2f) };
            _lines[40] = new[] { F(.5f, 0f, .5f, 1f), F(0f, .5f, 1f, .5f) };

            // 41: Concentric circles ◎
            _circles[41] = new[] { F(.5f, .5f, .4f), F(.5f, .5f, .2f) };

            // 42: Sun — circle with 8 short rays ☉
            _circles[42] = new[] { F(.5f, .5f, .2f) };
            _lines[42] = new[] {
                F(.5f, 0f, .5f, .28f), F(.5f, .72f, .5f, 1f),
                F(0f, .5f, .28f, .5f), F(.72f, .5f, 1f, .5f),
                F(.15f, .15f, .36f, .36f), F(.64f, .64f, .85f, .85f),
                F(.85f, .15f, .64f, .36f), F(.36f, .64f, .15f, .85f)
            };

            // 43: Six-pointed star ✡ (two overlapping triangles)
            _lines[43] = new[] {
                F(.5f, .05f, .82f, .75f, .18f, .75f, .5f, .05f),
                F(.5f, .95f, .18f, .25f, .82f, .25f, .5f, .95f)
            };

            // 44: Square with internal cross ⊞
            _lines[44] = new[] {
                F(0f, 0f, 1f, 0f, 1f, 1f, 0f, 1f, 0f, 0f),
                F(.5f, 0f, .5f, 1f), F(0f, .5f, 1f, .5f)
            };

            // ── 45–49: Complex / alien ────────────────────────────────
            // 45: Four corner brackets ⌐
            _lines[45] = new[] {
                F(0f, .3f, 0f, 0f, .3f, 0f),
                F(.7f, 0f, 1f, 0f, 1f, .3f),
                F(0f, .7f, 0f, 1f, .3f, 1f),
                F(.7f, 1f, 1f, 1f, 1f, .7f)
            };

            // 46: Crescent moon ☽
            _lines[46] = new[] {
                F(.72f, .08f, .42f, .22f, .25f, .5f, .42f, .78f, .72f, .92f,
                  .58f, .72f, .52f, .5f, .58f, .28f, .72f, .08f)
            };

            // 47: Infinity / figure-8 ∞
            _lines[47] = new[] {
                F(.5f, .5f, .32f, .22f, .12f, .3f, .12f, .7f, .32f, .78f, .5f, .5f,
                  .68f, .22f, .88f, .3f, .88f, .7f, .68f, .78f, .5f, .5f)
            };

            // 48: Dot-line-dot (⊶)
            _dots[48] = new[] { F(.12f, .5f, .1f), F(.88f, .5f, .1f) };
            _lines[48] = new[] { F(.24f, .5f, .76f, .5f) };

            // 49: Alien face 👽
            _circles[49] = new[] { F(.5f, .42f, .38f) };
            _dots[49] = new[] { F(.34f, .32f, .08f), F(.66f, .32f, .08f) };
            _lines[49] = new[] { F(.35f, .58f, .5f, .68f, .65f, .58f) };
        }
    }
}