// Adapted from Simon Tatham's Portable Puzzles Collection (Mines Puzzle)

using System;
using System.Collections.Generic;
using System.Linq;

public class MinesweeperGenerator
{
    // Represents a logical deduction set: a 3x3 area with a known number of mines.
    public class MineSet : IComparable<MineSet>
    {
        public int X, Y, Mask, Mines;

        public MineSet(int x, int y, int mask, int mines)
        {
            X = x; Y = y; Mask = mask; Mines = mines;
        }

        // Sorting order mirrors the original C tree234 comparison
        public int CompareTo(MineSet other)
        {
            if (Y != other.Y) return Y.CompareTo(other.Y);
            if (X != other.X) return X.CompareTo(other.X);
            return Mask.CompareTo(other.Mask);
        }
    }

    private int _width;
    private int _height;
    private int _totalMines;
    private Random _rng;

    public MinesweeperGenerator(int width, int height, int mines, int seed = -1)
    {
        _width = width;
        _height = height;
        _totalMines = mines;
        _rng = seed == -1 ? new Random() : new Random(seed);
    }

    /// <summary>
    /// Main entry point. Generates boards until it finds one that can be solved without guessing.
    /// </summary>
    public bool[] GenerateSolvableBoard(int startX, int startY, int maxAttempts = 1000)
    {
        bool[] finalBoard = null;
        bool success = false;
        int attempts = 0;

        while (!success && attempts < maxAttempts)
        {
            attempts++;
            finalBoard = GenerateRandomBoard(startX, startY);
            
            // Create a mock grid for the solver (-2 = unknown, -1 = flagged mine, 0-8 = opened count)
            int[] solverGrid = new int[_width * _height];
            for (int i = 0; i < solverGrid.Length; i++) solverGrid[i] = -2;

            // Open the starting square
            OpenSquare(startX, startY, finalBoard, solverGrid);

            // Run the logic solver
            success = Solve(solverGrid, finalBoard);

            // Note: Tatham's full algorithm triggers "MinePerturb" here if it fails, 
            // swapping mines to fix the board instead of throwing it away. 
            // For brevity, throwing away and retrying is often fast enough for standard board sizes.
        }

        if (success)
            Console.WriteLine($"Success! Solvable board generated in {attempts} attempts.");
        else
            Console.WriteLine($"Failed to generate purely solvable board after {maxAttempts} attempts. Returning best effort.");

        return finalBoard;
    }

    /// <summary>
    /// Randomly places mines, ensuring the start square and its 8 neighbors are empty.
    /// </summary>
    private bool[] GenerateRandomBoard(int startX, int startY)
    {
        bool[] board = new bool[_width * _height];
        List<int> validPositions = new List<int>();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (Math.Abs(x - startX) > 1 || Math.Abs(y - startY) > 1)
                {
                    validPositions.Add(y * _width + x);
                }
            }
        }

        for (int i = 0; i < _totalMines; i++)
        {
            int r = _rng.Next(validPositions.Count);
            board[validPositions[r]] = true;
            validPositions[r] = validPositions[validPositions.Count - 1];
            validPositions.RemoveAt(validPositions.Count - 1);
        }

        return board;
    }

    /// <summary>
    /// The logic solver. Returns true if the board can be solved using pure logic.
    /// </summary>
    private bool Solve(int[] solverGrid, bool[] actualBoard)
    {
        int maxPerturbs = 100;
        int perturbs = 0;

        while (true)
        {
            bool doneSomething = true;

            while (doneSomething)
            {
                doneSomething = false;

                // Step 1: Rebuild current sets dynamically from the grid
                List<MineSet> sets = new List<MineSet>();
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        int idx = y * _width + x;
                        if (solverGrid[idx] >= 0) // It's a revealed number
                        {
                            int mask = 0;
                            int mines = solverGrid[idx];
                            int bit = 1;

                            for (int dy = -1; dy <= 1; dy++)
                            {
                                for (int dx = -1; dx <= 1; dx++)
                                {
                                    int nx = x + dx, ny = y + dy;
                                    if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
                                    {
                                        int nIdx = ny * _width + nx;
                                        if (solverGrid[nIdx] == -1) mines--; // Deduct flagged neighbors
                                        else if (solverGrid[nIdx] == -2) mask |= bit; // Record unknown neighbors
                                    }
                                    bit <<= 1;
                                }
                            }

                            if (mask > 0)
                            {
                                MineSet newSet = new MineSet(x - 1, y - 1, mask, mines);
                                sets.Add(newSet);

                                // Basic Logic: If set requires 0 mines, or all remaining squares are mines
                                if (mines == 0 || mines == BitCount(mask))
                                {
                                    ResolveSet(newSet, mines > 0, actualBoard, solverGrid);
                                    doneSomething = true;
                                }
                            }
                        }
                    }
                }

                // If Basic Logic found something, restart the loop to rebuild sets with the fresh data!
                if (doneSomething) continue;

                // Step 2: Advanced Intersection Logic (Using SetMunge)
                for (int i = 0; i < sets.Count; i++)
                {
                    for (int j = i + 1; j < sets.Count; j++)
                    {
                        MineSet A = sets[i];
                        MineSet B = sets[j];

                        // Skip if sets are too far apart to overlap
                        if (Math.Abs(A.X - B.X) >= 3 || Math.Abs(A.Y - B.Y) >= 3) continue;

                        // Calculate the non-overlapping "wings" (A minus B, and B minus A)
                        int A_minus_B = SetMunge(A.X, A.Y, A.Mask, B.X, B.Y, B.Mask, true);
                        int B_minus_A = SetMunge(B.X, B.Y, B.Mask, A.X, A.Y, A.Mask, true);
                        
                        int countA = BitCount(A_minus_B);
                        int countB = BitCount(B_minus_A);

                        // If the extra mines in A exactly match the unshared squares in A...
                        if (countA > 0 && countA == A.Mines - B.Mines)
                        {
                            if (A_minus_B > 0) ResolveSet(new MineSet(A.X, A.Y, A_minus_B, countA), true, actualBoard, solverGrid);
                            if (B_minus_A > 0) ResolveSet(new MineSet(B.X, B.Y, B_minus_A, 0), false, actualBoard, solverGrid);
                            doneSomething = true;
                            break; 
                        }
                        // Or if the extra mines in B exactly match the unshared squares in B...
                        else if (countB > 0 && countB == B.Mines - A.Mines)
                        {
                            if (B_minus_A > 0) ResolveSet(new MineSet(B.X, B.Y, B_minus_A, countB), true, actualBoard, solverGrid);
                            if (A_minus_B > 0) ResolveSet(new MineSet(A.X, A.Y, A_minus_B, 0), false, actualBoard, solverGrid);
                            doneSomething = true;
                            break;
                        }
                        
                        // Step 3: Subset Logic
                        // If A is completely inside B, we can create a new virtual deduction set!
                        if (countA == 0 && countB > 0) 
                        {
                            MineSet virtualSet = new MineSet(B.X, B.Y, B_minus_A, B.Mines - A.Mines);
                            
                            // Check if this new virtual set instantly solves anything
                            if (virtualSet.Mines == 0 || virtualSet.Mines == BitCount(virtualSet.Mask))
                            {
                                ResolveSet(virtualSet, virtualSet.Mines > 0, actualBoard, solverGrid);
                                doneSomething = true;
                                break;
                            }
                        }
                    }
                    if (doneSomething) break;
                }
            }

            // --- END LOGIC ---

            // Are there any -2 (unknown) tiles left?
            if (!solverGrid.Contains(-2)) return true; // Yes! The board is perfectly solvable!

            // --- GLOBAL DEDUCTION ---
            // If local logic fails, zoom out and look at the whole board.
            int squaresLeft = solverGrid.Count(s => s == -2);
            int minesLeft = _totalMines - solverGrid.Count(s => s == -1);

            if (squaresLeft > 0)
            {
                if (minesLeft == 0)
                {
                    // We found all the mines! Every other unknown tile is safe.
                    for (int i = 0; i < solverGrid.Length; i++)
                    {
                        if (solverGrid[i] == -2) OpenSquare(i % _width, i / _width, actualBoard, solverGrid);
                    }
                    continue; // Restart the logic loop with this new info!
                }
                else if (minesLeft == squaresLeft)
                {
                    // Every remaining unknown tile MUST be a mine!
                    for (int i = 0; i < solverGrid.Length; i++)
                    {
                        if (solverGrid[i] == -2) solverGrid[i] = -1; // Flag it
                    }
                    continue; // Restart the logic loop with this new info!
                }
            }
            // --- END GLOBAL DEDUCTION ---

            // The advanced logic AND global deduction got completely stuck. Trigger the Perturb fallback!
            if (perturbs < maxPerturbs && Perturb(solverGrid, actualBoard))
            {
                perturbs++;
            }
            else
            {
                // We ran out of perturbs, or the board is completely doomed. Throw it away.
                return false; 
            }
        }
    }


    /// <summary>
    /// Applies a solved set to the grid (either flagging mines or opening safes).
    /// </summary>
    private void ResolveSet(MineSet s, bool isMine, bool[] actualBoard, int[] solverGrid)
    {
        int bit = 1;
        for (int dy = 0; dy < 3; dy++)
        {
            for (int dx = 0; dx < 3; dx++)
            {
                if ((s.Mask & bit) != 0)
                {
                    int nx = s.X + dx, ny = s.Y + dy;
                    int idx = ny * _width + nx;

                    // Only act if the solver hasn't already figured this tile out
                    if (solverGrid[idx] == -2)
                    {
                        if (isMine)
                        {
                            solverGrid[idx] = -1; // Flag mine
                        }
                        else
                        {
                            OpenSquare(nx, ny, actualBoard, solverGrid); // Open safe space
                        }
                    }
                }
                bit <<= 1; // Shift to check the next bit in the 3x3 mask
            }
        }
    }

    /// <summary>
    /// Opens a square and calculates its neighbor mine count.
    /// </summary>
    private void OpenSquare(int x, int y, bool[] actualBoard, int[] solverGrid)
    {
        int count = 0;
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (nx < 0 || nx >= _width || ny < 0 || ny >= _height) continue;
                if (actualBoard[ny * _width + nx]) count++;
            }
        }
        solverGrid[y * _width + x] = count;
    }


    /// <summary>
    /// Translates Tatham's bitwise set alignment logic. 
    /// Aligns mask2 (from x2,y2) to overlap perfectly with mask1 (from x1,y1).
    /// </summary>
    private int SetMunge(int x1, int y1, int mask1, int x2, int y2, int mask2, bool diff)
    {
        if (Math.Abs(x2 - x1) >= 3 || Math.Abs(y2 - y1) >= 3) return 0;

        while (x2 > x1) { mask2 &= ~(4 | 32 | 256); mask2 <<= 1; x2--; }
        while (x2 < x1) { mask2 &= ~(1 | 8 | 64); mask2 >>= 1; x2++; }
        while (y2 > y1) { mask2 &= ~(64 | 128 | 256); mask2 <<= 3; y2--; }
        while (y2 < y1) { mask2 &= ~(1 | 2 | 4); mask2 >>= 3; y2++; }

        if (diff) mask2 ^= 511;

        return mask1 & mask2;
    }

    /// <summary>
    /// Counts the number of active bits in the 9-bit mask.
    /// </summary>
    private int BitCount(int mask)
    {
        int count = 0;
        while (mask > 0)
        {
            count += mask & 1;
            mask >>= 1;
        }
        return count;
    }

    /// <summary>
    /// Adapts Tatham's "Global" Perturb. 
    /// Swaps a mine and an empty space within the unresolved area to break deadlocks.
    /// </summary>
    private bool Perturb(int[] solverGrid, bool[] actualBoard)
    {
        List<int> unknownMines = new List<int>();
        List<int> unknownSafes = new List<int>();

        // 1. Gather all squares the solver hasn't figured out yet
        for (int i = 0; i < solverGrid.Length; i++)
        {
            if (solverGrid[i] == -2)
            {
                if (actualBoard[i]) unknownMines.Add(i);
                else unknownSafes.Add(i);
            }
        }

        // If we don't have at least one mine and one safe space to swap, perturb fails.
        if (unknownMines.Count == 0 || unknownSafes.Count == 0) return false;

        // 2. Pick a random mine and a random safe space from the unknown area
        int mineIdx = unknownMines[_rng.Next(unknownMines.Count)];
        int safeIdx = unknownSafes[_rng.Next(unknownSafes.Count)];

        // 3. Swap them on the actual physical board
        actualBoard[mineIdx] = false;
        actualBoard[safeIdx] = true;

        // 4. Update the solver's revealed numbers (0-8) that touch the swapped tiles
        UpdateSolverNumbersAround(mineIdx, actualBoard, solverGrid);
        UpdateSolverNumbersAround(safeIdx, actualBoard, solverGrid);

        return true;
    }

    /// <summary>
    /// When we move a mine, we must update the numbers of any ALREADY REVEALED tiles that touch it.
    /// </summary>
    private void UpdateSolverNumbersAround(int index, bool[] actualBoard, int[] solverGrid)
    {
        int x = index % _width;
        int y = index / _width;

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
                {
                    int nIdx = ny * _width + nx;
                    
                    // Only update tiles that the solver has actually opened (0 to 8)
                    if (solverGrid[nIdx] >= 0)
                    {
                        int count = 0;
                        // Recalculate neighbors for this open tile
                        for (int ndy = -1; ndy <= 1; ndy++)
                        {
                            for (int ndx = -1; ndx <= 1; ndx++)
                            {
                                int nnx = nx + ndx, nny = ny + ndy;
                                if (nnx >= 0 && nnx < _width && nny >= 0 && nny < _height)
                                {
                                    if (actualBoard[nny * _width + nnx]) count++;
                                }
                            }
                        }
                        solverGrid[nIdx] = count;
                    }
                }
            }
        }
    }
}
