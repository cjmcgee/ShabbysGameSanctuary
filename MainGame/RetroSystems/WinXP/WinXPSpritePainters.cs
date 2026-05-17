using ChildhoodAdventure.RetroSystems.Assets;

namespace ChildhoodAdventure.RetroSystems.WinXP;

/// <summary>
/// Procedural painters for WinXP character sprite art. Each routine writes
/// pixels into the shared sprite-sheet buffer using only the semantic marker
/// colors from <see cref="SemanticPalette"/>, so the resulting PNG can be
/// loaded back into byte[][] semantic-index art by <see cref="AssetLoader"/>
/// and recoloured per-character through the active HeadPalette/BodyPalette/
/// LegsPalette.
///
/// Character dimensions (16 wide × 32 tall total):
///   • Head: 8 rows
///   • Body: 12 rows
///   • Legs: 12 rows (× 4 walk frames)
/// </summary>
internal static class WinXPSpritePainters
{
	// Semantic markers re-aliased for legibility.
	private static readonly Color Skin    = SemanticPalette.SlotSkin;
	private static readonly Color Hair    = SemanticPalette.SlotHair;
	private static readonly Color HL      = SemanticPalette.SlotHighlight;
	private static readonly Color Eyes    = SemanticPalette.SlotEyes;
	private static readonly Color Acc     = SemanticPalette.SlotAccessory;
	private static readonly Color Shoes   = SemanticPalette.SlotShoes;
	private static readonly Color ShoeHi  = SemanticPalette.SlotShoeHi;

	// Body / legs reuse the same color slots for their respective semantics:
	//   Body: Skin=neck/face, Hair=Shirt, HL=ShirtHighlight, Eyes=Buttons, Acc=Accessory
	//   Legs: Skin=bare skin, Hair=Pants, HL=PantsHighlight, Eyes=Belt,
	//         Acc=BeltHighlight, Shoes=Shoes, ShoeHi=ShoeHighlight
	// Painters below choose markers based on which part is being drawn.

	private const int W = WinXPAssetSeed.CharWidth;	// 16

	private static void Plot(Color[] buf, int stride, int x0, int y0, int x, int y, Color c)
	{
		buf[(y0 + y) * stride + (x0 + x)] = c;
	}

	private static void Row(Color[] buf, int stride, int x0, int y0, int y, ReadOnlySpan<Color?> cells)
	{
		for (int x = 0; x < cells.Length && x < W; x++)
		{
			if (cells[x] is Color c) Plot(buf, stride, x0, y0, x, y, c);
		}
	}

	// Shorthand for laying out a row from a string template:
	//   '.'  = transparent (skip)
	//   's'  = Skin / neck
	//   'h'  = Hair / Shirt / Pants (slot 2 — the dominant non-skin color)
	//   'L'  = Highlight (slot 3)
	//   'e'  = Eyes / Buttons / Belt (slot 4)
	//   'a'  = Accessory / BeltHighlight (slot 5)
	//   'o'  = Shoes (slot 6)
	//   'O'  = ShoeHighlight (slot 7)
	private static void RowT(Color[] buf, int stride, int x0, int y0, int y, string template)
	{
		for (int x = 0; x < template.Length && x < W; x++)
		{
			Color? c = template[x] switch
			{
				's' => Skin,
				'h' => Hair,
				'L' => HL,
				'e' => Eyes,
				'a' => Acc,
				'o' => Shoes,
				'O' => ShoeHi,
				_   => null,
			};
			if (c is Color cc) Plot(buf, stride, x0, y0, x, y, cc);
		}
	}

	// ── Heads (8 rows × 16 cols) ──────────────────────────────────────────

	public static void PaintHead(int variant, Facing facing, Color[] buf, int stride, int x0, int y0)
	{
		switch ((variant, facing))
		{
			case (0, Facing.Front): Head0Front(buf, stride, x0, y0); break;
			case (0, Facing.Back):  Head0Back(buf, stride, x0, y0);  break;
			case (0, Facing.Side):  Head0Side(buf, stride, x0, y0);  break;
			case (1, Facing.Front): Head1Front(buf, stride, x0, y0); break;
			case (1, Facing.Back):  Head1Back(buf, stride, x0, y0);  break;
			case (1, Facing.Side):  Head1Side(buf, stride, x0, y0);  break;
			case (2, Facing.Front): Head2Front(buf, stride, x0, y0); break;
			case (2, Facing.Back):  Head2Back(buf, stride, x0, y0);  break;
			case (2, Facing.Side):  Head2Side(buf, stride, x0, y0);  break;
		}
	}

	// Basic short hair, oval head.
	private static void Head0Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, ".....hhhhhh.....");
		RowT(buf, stride, x0, y0, 1, "....hhhhhhhh....");
		RowT(buf, stride, x0, y0, 2, "...hsssssssh....");
		RowT(buf, stride, x0, y0, 3, "...hseseess.h...");	// raw eyes
		RowT(buf, stride, x0, y0, 4, "....sssssss.....");
		RowT(buf, stride, x0, y0, 5, "....sLssLss.....");
		RowT(buf, stride, x0, y0, 6, ".....ssss.......");
		RowT(buf, stride, x0, y0, 7, "......ss........");
	}

	private static void Head0Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, ".....hhhhhh.....");
		RowT(buf, stride, x0, y0, 1, "....hhhhhhhh....");
		RowT(buf, stride, x0, y0, 2, "...hhhhhhhhh....");
		RowT(buf, stride, x0, y0, 3, "...hhhhhhhhh....");
		RowT(buf, stride, x0, y0, 4, "....sssssss.....");
		RowT(buf, stride, x0, y0, 5, "....sssssss.....");
		RowT(buf, stride, x0, y0, 6, ".....ssss.......");
		RowT(buf, stride, x0, y0, 7, "......ss........");
	}

	private static void Head0Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "....hhhhh.......");
		RowT(buf, stride, x0, y0, 1, "...hhhhhhh......");
		RowT(buf, stride, x0, y0, 2, "..hsssseh.......");
		RowT(buf, stride, x0, y0, 3, "..hsssssh.......");
		RowT(buf, stride, x0, y0, 4, "...sssss........");
		RowT(buf, stride, x0, y0, 5, "...sLsss........");
		RowT(buf, stride, x0, y0, 6, "....sss.........");
		RowT(buf, stride, x0, y0, 7, "....ss..........");
	}

	// Cap / hat variant.
	private static void Head1Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "....aaaaaaaa....");
		RowT(buf, stride, x0, y0, 1, "...aaaaaaaaaa...");
		RowT(buf, stride, x0, y0, 2, "..aaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 3, "...ssesssssess..");
		RowT(buf, stride, x0, y0, 4, "....sssssss.....");
		RowT(buf, stride, x0, y0, 5, "....sLsssLs.....");
		RowT(buf, stride, x0, y0, 6, ".....ssss.......");
		RowT(buf, stride, x0, y0, 7, "......ss........");
	}

	private static void Head1Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "....aaaaaaaa....");
		RowT(buf, stride, x0, y0, 1, "...aaaaaaaaaa...");
		RowT(buf, stride, x0, y0, 2, "..aaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 3, "...ssssssssss...");
		RowT(buf, stride, x0, y0, 4, "....sssssss.....");
		RowT(buf, stride, x0, y0, 5, "....sssssss.....");
		RowT(buf, stride, x0, y0, 6, ".....ssss.......");
		RowT(buf, stride, x0, y0, 7, "......ss........");
	}

	private static void Head1Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "...aaaaaa.......");
		RowT(buf, stride, x0, y0, 1, "..aaaaaaaaa.....");
		RowT(buf, stride, x0, y0, 2, ".aaaaaaaaaa.....");
		RowT(buf, stride, x0, y0, 3, "...sseeeesh.....");
		RowT(buf, stride, x0, y0, 4, "...ssssss.......");
		RowT(buf, stride, x0, y0, 5, "...sLssss.......");
		RowT(buf, stride, x0, y0, 6, "....ssss........");
		RowT(buf, stride, x0, y0, 7, "....ss..........");
	}

	// Long / full hair variant.
	private static void Head2Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "....hhhhhhhh....");
		RowT(buf, stride, x0, y0, 1, "...hhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 2, "..hhhsssssshh...");
		RowT(buf, stride, x0, y0, 3, "..hhseesseshh...");
		RowT(buf, stride, x0, y0, 4, "..hsssssssssh...");
		RowT(buf, stride, x0, y0, 5, "..hsLsssssLsh...");
		RowT(buf, stride, x0, y0, 6, "...hsssssssh....");
		RowT(buf, stride, x0, y0, 7, "....hhsshh......");
	}

	private static void Head2Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "....hhhhhhhh....");
		RowT(buf, stride, x0, y0, 1, "...hhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 2, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 3, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 4, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 5, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 6, "...hhhsshhhh....");
		RowT(buf, stride, x0, y0, 7, "....hhsshh......");
	}

	private static void Head2Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "...hhhhhh.......");
		RowT(buf, stride, x0, y0, 1, "..hhhhhhhh......");
		RowT(buf, stride, x0, y0, 2, ".hhsssseh.......");
		RowT(buf, stride, x0, y0, 3, ".hhssssshh......");
		RowT(buf, stride, x0, y0, 4, ".hssssshhh......");
		RowT(buf, stride, x0, y0, 5, ".hsLssshhh......");
		RowT(buf, stride, x0, y0, 6, "..hssshhhh......");
		RowT(buf, stride, x0, y0, 7, "...hhhhh........");
	}

	// ── Bodies (12 rows × 16 cols) ────────────────────────────────────────

	public static void PaintBody(int variant, Facing facing, Color[] buf, int stride, int x0, int y0)
	{
		switch ((variant, facing))
		{
			case (0, Facing.Front): Body0Front(buf, stride, x0, y0); break;
			case (0, Facing.Back):  Body0Back(buf, stride, x0, y0);  break;
			case (0, Facing.Side):  Body0Side(buf, stride, x0, y0);  break;
			case (1, Facing.Front): Body1Front(buf, stride, x0, y0); break;
			case (1, Facing.Back):  Body1Back(buf, stride, x0, y0);  break;
			case (1, Facing.Side):  Body1Side(buf, stride, x0, y0);  break;
			case (2, Facing.Front): Body2Front(buf, stride, x0, y0); break;
			case (2, Facing.Back):  Body2Back(buf, stride, x0, y0);  break;
			case (2, Facing.Side):  Body2Side(buf, stride, x0, y0);  break;
		}
	}

	// Casual T-shirt
	private static void Body0Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...hhhsshhh.....");
		RowT(buf, stride, x0, y0, 3, "..hhhhsshhhh....");
		RowT(buf, stride, x0, y0, 4, ".hhhLhhhhLhhh...");
		RowT(buf, stride, x0, y0, 5, ".shhhhheehhhs...");
		RowT(buf, stride, x0, y0, 6, ".shhhLheehLhhs..");
		RowT(buf, stride, x0, y0, 7, "..hhhheehhhhh...");
		RowT(buf, stride, x0, y0, 8, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 9, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,10, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,11, "..hhhhhhhhhh....");
	}

	private static void Body0Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...hhhhhhhhh....");
		RowT(buf, stride, x0, y0, 3, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 4, ".hhhLhhhhhLhhh..");
		RowT(buf, stride, x0, y0, 5, ".shhhhhhhhhhhs..");
		RowT(buf, stride, x0, y0, 6, ".shhhhhhhhhhhs..");
		RowT(buf, stride, x0, y0, 7, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 8, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 9, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,10, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,11, "..hhhhhhhhhh....");
	}

	private static void Body0Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, ".....ss.........");
		RowT(buf, stride, x0, y0, 1, "....ssss........");
		RowT(buf, stride, x0, y0, 2, "...hhhhhh.......");
		RowT(buf, stride, x0, y0, 3, "..hhhhhhh.......");
		RowT(buf, stride, x0, y0, 4, ".hhLhhhh........");
		RowT(buf, stride, x0, y0, 5, ".hhhhheh........");
		RowT(buf, stride, x0, y0, 6, ".hhLhheh........");
		RowT(buf, stride, x0, y0, 7, "..hhhheh........");
		RowT(buf, stride, x0, y0, 8, "..hhhhhh........");
		RowT(buf, stride, x0, y0, 9, "..hhhhhh........");
		RowT(buf, stride, x0, y0,10, "..hhhhhh........");
		RowT(buf, stride, x0, y0,11, "..hhhhhh........");
	}

	// Formal / button-down
	private static void Body1Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...hhaaaahhh....");
		RowT(buf, stride, x0, y0, 3, "..hhaahhaahh....");
		RowT(buf, stride, x0, y0, 4, ".hhhahhhhahhh...");
		RowT(buf, stride, x0, y0, 5, ".shhhheehhhhs...");
		RowT(buf, stride, x0, y0, 6, ".shhhheehhhhs...");
		RowT(buf, stride, x0, y0, 7, "..hhhheehhhh....");
		RowT(buf, stride, x0, y0, 8, "..hhhheehhhh....");
		RowT(buf, stride, x0, y0, 9, "..hhhheehhhh....");
		RowT(buf, stride, x0, y0,10, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,11, "..hhhhhhhhhh....");
	}

	private static void Body1Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...hhaaaahhh....");
		RowT(buf, stride, x0, y0, 3, "..hhaaaaaaahh...");
		RowT(buf, stride, x0, y0, 4, ".hhhaaaaaaaahh..");
		RowT(buf, stride, x0, y0, 5, ".shhhhhhhhhhhs..");
		RowT(buf, stride, x0, y0, 6, ".shhhhhhhhhhhs..");
		RowT(buf, stride, x0, y0, 7, "..hhhhhhhhhhh...");
		RowT(buf, stride, x0, y0, 8, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 9, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,10, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0,11, "..hhhhhhhhhh....");
	}

	private static void Body1Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, ".....ss.........");
		RowT(buf, stride, x0, y0, 1, "....ssss........");
		RowT(buf, stride, x0, y0, 2, "...hhaahh.......");
		RowT(buf, stride, x0, y0, 3, "..hhahhhh.......");
		RowT(buf, stride, x0, y0, 4, ".hhhahhhh.......");
		RowT(buf, stride, x0, y0, 5, ".hhhhheh........");
		RowT(buf, stride, x0, y0, 6, ".hhhhheh........");
		RowT(buf, stride, x0, y0, 7, "..hhhheh........");
		RowT(buf, stride, x0, y0, 8, "..hhhheh........");
		RowT(buf, stride, x0, y0, 9, "..hhhhhh........");
		RowT(buf, stride, x0, y0,10, "..hhhhhh........");
		RowT(buf, stride, x0, y0,11, "..hhhhhh........");
	}

	// Jacket / hoodie (uses Accessory color for outer jacket, Hair for inner shirt)
	private static void Body2Front(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...aaasshhha....");
		RowT(buf, stride, x0, y0, 3, "..aaahhsshhaa...");
		RowT(buf, stride, x0, y0, 4, ".aaaahhhhhhaaa..");
		RowT(buf, stride, x0, y0, 5, ".aahhhheehhhaa..");
		RowT(buf, stride, x0, y0, 6, ".aahhLheehLhaa..");
		RowT(buf, stride, x0, y0, 7, ".aahhhheehhhaa..");
		RowT(buf, stride, x0, y0, 8, ".aahhhhhhhhhaa..");
		RowT(buf, stride, x0, y0, 9, ".aahhhhhhhhhaa..");
		RowT(buf, stride, x0, y0,10, ".aahhhhhhhhhaa..");
		RowT(buf, stride, x0, y0,11, ".aaaaaaaaaaaaa..");
	}

	private static void Body2Back(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "......ss........");
		RowT(buf, stride, x0, y0, 1, ".....ssss.......");
		RowT(buf, stride, x0, y0, 2, "...aaaaaaaaa....");
		RowT(buf, stride, x0, y0, 3, "..aaaaaaaaaaa...");
		RowT(buf, stride, x0, y0, 4, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 5, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 6, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 7, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 8, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0, 9, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0,10, ".aaaaaaaaaaaaa..");
		RowT(buf, stride, x0, y0,11, ".aaaaaaaaaaaaa..");
	}

	private static void Body2Side(Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, ".....ss.........");
		RowT(buf, stride, x0, y0, 1, "....ssss........");
		RowT(buf, stride, x0, y0, 2, "...aaaaaa.......");
		RowT(buf, stride, x0, y0, 3, "..aaahhha.......");
		RowT(buf, stride, x0, y0, 4, ".aaahhhha.......");
		RowT(buf, stride, x0, y0, 5, ".aahhhhea.......");
		RowT(buf, stride, x0, y0, 6, ".aahhhhea.......");
		RowT(buf, stride, x0, y0, 7, ".aahhhhea.......");
		RowT(buf, stride, x0, y0, 8, ".aahhhhha.......");
		RowT(buf, stride, x0, y0, 9, ".aahhhhha.......");
		RowT(buf, stride, x0, y0,10, ".aahhhhha.......");
		RowT(buf, stride, x0, y0,11, ".aaaaaaaa.......");
	}

	// ── Legs (12 rows × 16 cols, 4 walk frames) ───────────────────────────

	public static void PaintLegs(int variant, Facing facing, int frame, Color[] buf, int stride, int x0, int y0)
	{
		switch ((variant, facing))
		{
			case (0, Facing.Front): Legs0Front(frame, buf, stride, x0, y0); break;
			case (0, Facing.Side):  Legs0Side(frame, buf, stride, x0, y0);  break;
			case (1, Facing.Front): Legs1Front(frame, buf, stride, x0, y0); break;
			case (1, Facing.Side):  Legs1Side(frame, buf, stride, x0, y0);  break;
			case (2, Facing.Front): Legs2Front(frame, buf, stride, x0, y0); break;
			case (2, Facing.Side):  Legs2Side(frame, buf, stride, x0, y0);  break;
		}
	}

	// Pants + belt (variant 0).
	private static void Legs0Front(int f, Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "..eeeeeeeeee....");	// belt
		RowT(buf, stride, x0, y0, 1, "..hhaaaaaahh....");	// belt buckle
		RowT(buf, stride, x0, y0, 2, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 3, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 4, "..hhhLhhLhhh....");
		string r5 = f switch
		{
			1 => ".hhhh....hhhh...",	// step left
			2 => "..hhhh..hhhh....",
			3 => "..hhhh....hhhh.",	// step right
			_ => "..hhhh..hhhh....",
		};
		RowT(buf, stride, x0, y0, 5, r5);
		string r6 = f switch
		{
			1 => "hhhh.....hhhh...",
			3 => "..hhhh.....hhhh.",
			_ => "..hhh....hhh....",
		};
		RowT(buf, stride, x0, y0, 6, r6);
		string r7 = f switch
		{
			1 => "hhh......hhh....",
			3 => "...hhh......hhh.",
			_ => "..hhh....hhh....",
		};
		RowT(buf, stride, x0, y0, 7, r7);
		string r8 = f switch
		{
			1 => "hhh......hhh....",
			3 => "...hhh......hhh.",
			_ => "..hhh....hhh....",
		};
		RowT(buf, stride, x0, y0, 8, r8);
		string r9 = f switch
		{
			1 => "hhh......hhh....",
			3 => "...hhh......hhh.",
			_ => "..hhh....hhh....",
		};
		RowT(buf, stride, x0, y0, 9, r9);
		string r10 = f switch
		{
			1 => "ooo......ooo....",
			3 => "...ooo......ooo.",
			_ => "..ooo....ooo....",
		};
		RowT(buf, stride, x0, y0, 10, r10);
		string r11 = f switch
		{
			1 => "ooO......ooO....",
			3 => "...Ooo......Ooo.",
			_ => "..oOO....OOo....",
		};
		RowT(buf, stride, x0, y0, 11, r11);
	}

	private static void Legs0Side(int f, Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "..eeeeee........");
		RowT(buf, stride, x0, y0, 1, "..hhaaah........");
		RowT(buf, stride, x0, y0, 2, "..hhhhhh........");
		RowT(buf, stride, x0, y0, 3, "..hhhhhh........");
		RowT(buf, stride, x0, y0, 4, "..hhhhhh........");
		// Walk cycle: alternating leg extension forward / back.
		string r5 = f switch { 1 => ".hhhhhhh........", 3 => "...hhhhhh.......", _ => "..hhhhhh........" };
		string r6 = f switch { 1 => "hhh...hhh.......", 3 => "....hhh...hhh...", _ => "..hh....hh......" };
		string r7 = f switch { 1 => "hh.....hh.......", 3 => ".....hh.....hh..", _ => "..hh....hh......" };
		string r8 = f switch { 1 => "hh.....hh.......", 3 => ".....hh.....hh..", _ => "..hh....hh......" };
		string r9 = f switch { 1 => "hh.....hh.......", 3 => ".....hh.....hh..", _ => "..hh....hh......" };
		string r10 = f switch { 1 => "oo.....oo.......", 3 => ".....oo.....oo..", _ => "..oo....oo......" };
		string r11 = f switch { 1 => "oO.....oO.......", 3 => ".....Oo.....Oo..", _ => "..oO....Oo......" };
		RowT(buf, stride, x0, y0, 5, r5);
		RowT(buf, stride, x0, y0, 6, r6);
		RowT(buf, stride, x0, y0, 7, r7);
		RowT(buf, stride, x0, y0, 8, r8);
		RowT(buf, stride, x0, y0, 9, r9);
		RowT(buf, stride, x0, y0, 10, r10);
		RowT(buf, stride, x0, y0, 11, r11);
	}

	// Formal trousers — adds crease lines (variant 1).
	private static void Legs1Front(int f, Color[] buf, int stride, int x0, int y0)
	{
		Legs0Front(f, buf, stride, x0, y0);
		// Replace centre lines with highlight strands for the crease.
		Plot(buf, stride, x0, y0, 4, 5, HL);
		Plot(buf, stride, x0, y0, 9, 5, HL);
		Plot(buf, stride, x0, y0, 4, 7, HL);
		Plot(buf, stride, x0, y0, 9, 7, HL);
		Plot(buf, stride, x0, y0, 4, 9, HL);
		Plot(buf, stride, x0, y0, 9, 9, HL);
	}

	private static void Legs1Side(int f, Color[] buf, int stride, int x0, int y0)
	{
		Legs0Side(f, buf, stride, x0, y0);
		// Crease down each visible leg pillar.
		Plot(buf, stride, x0, y0, 3, 6, HL);
		Plot(buf, stride, x0, y0, 3, 8, HL);
	}

	// Shorts + bare skin (variant 2).
	private static void Legs2Front(int f, Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "..eeeeeeeeee....");
		RowT(buf, stride, x0, y0, 1, "..hhaaaaaahh....");
		RowT(buf, stride, x0, y0, 2, "..hhhhhhhhhh....");
		RowT(buf, stride, x0, y0, 3, "..hhhhhhhhhh....");	// short pants end here
		string r4 = f switch
		{
			1 => ".ssss....ssss...",
			3 => "..ssss......ssss",
			_ => "..ssss..ssss....",
		};
		RowT(buf, stride, x0, y0, 4, r4);
		string r5 = f switch
		{
			1 => "ssss.....ssss...",
			3 => "...ssss......sss",
			_ => "..sss....sss....",
		};
		RowT(buf, stride, x0, y0, 5, r5);
		RowT(buf, stride, x0, y0, 6, r5);
		RowT(buf, stride, x0, y0, 7, r5);
		RowT(buf, stride, x0, y0, 8, r5);
		RowT(buf, stride, x0, y0, 9, r5);
		string r10 = f switch
		{
			1 => "ooo......ooo....",
			3 => "...ooo......ooo.",
			_ => "..ooo....ooo....",
		};
		RowT(buf, stride, x0, y0, 10, r10);
		string r11 = f switch
		{
			1 => "oOo......oOo....",
			3 => "...oOo......oOo.",
			_ => "..oOo....oOo....",
		};
		RowT(buf, stride, x0, y0, 11, r11);
	}

	private static void Legs2Side(int f, Color[] buf, int stride, int x0, int y0)
	{
		RowT(buf, stride, x0, y0, 0, "..eeeeee........");
		RowT(buf, stride, x0, y0, 1, "..hhaaah........");
		RowT(buf, stride, x0, y0, 2, "..hhhhhh........");
		RowT(buf, stride, x0, y0, 3, "..hhhhhh........");
		// Below shorts: bare skin then shoes.
		string r4 = f switch { 1 => ".sss...sss......", 3 => "...sss...sss....", _ => "..ss....ss......" };
		string r5 = f switch { 1 => "ss.....ss.......", 3 => ".....ss.....ss..", _ => "..ss....ss......" };
		RowT(buf, stride, x0, y0, 4, r4);
		RowT(buf, stride, x0, y0, 5, r5);
		RowT(buf, stride, x0, y0, 6, r5);
		RowT(buf, stride, x0, y0, 7, r5);
		RowT(buf, stride, x0, y0, 8, r5);
		RowT(buf, stride, x0, y0, 9, r5);
		string r10 = f switch { 1 => "oo.....oo.......", 3 => ".....oo.....oo..", _ => "..oo....oo......" };
		string r11 = f switch { 1 => "oO.....oO.......", 3 => ".....Oo.....Oo..", _ => "..oO....Oo......" };
		RowT(buf, stride, x0, y0, 10, r10);
		RowT(buf, stride, x0, y0, 11, r11);
	}
}
