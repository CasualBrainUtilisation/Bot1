using ChessChallenge.API;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;

public class MyBot : IChessBot
{

    // Every piece gets material value, each material on board will be valued by this
    public int[] MATERIAL_VALUE = 
    {
        000, //None
        100, //Pawn
        300, //Horsey
        300, //NonHorseyBoy
        500, //Rook
        900, //Queen
        000, //King
    };

    // Every piece gets a mobility value, having a lot of moves avaible will be rated by this
    public int[] MOBILITY_VALUE =
    {
        0, //None
        0, //Pawn
        5, //Horsey
        10, //NonHorseyBoy
        5, //Rook
        0, //Queen
       -2, //King
    };

    // Extra value of a check move
    public const int CHECK_VALUE = 50; // TODO: add

    // Extra value of a castle move
    public const int CASTLE_VALUE = 50;

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        Move moveToPerform = moves[0];

        int maxi(int depth, int a, int b, bool root = false)
        {
            if (depth == 0 || board.IsDraw() || board.IsInCheckmate()) return Evaluate(board);

            int max = -9999999;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int score = mini(depth - 1, a, b);

                if (score > max)
                {
                    max = score;
                    a = score;
                    if (root) moveToPerform = move;
                }
                board.UndoMove(move);
                if (b <= a) break;
            }
            return max;
        }

        int mini(int depth, int a, int b, bool root = false)
        {
            if (depth == 0 || board.IsDraw() || board.IsInCheckmate()) return Evaluate(board);

            int min = 9999999;
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                int score = maxi(depth - 1, a, b);

                if (score < min)
                {
                    min = score;
                    b = score;
                    if (root) moveToPerform = move;
                }
                board.UndoMove(move);
                if (b <= a) break;
            }
            return min;
        }

        if (board.IsWhiteToMove) maxi(3, -9999999, 9999999, true);
        else mini(3, -9999999, 9999999, true);

        return moveToPerform;
    }

    

    int Evaluate(Board board)
    {
        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -9999999 : 9999999;
        if (board.IsDraw() ) return 0;

        int value = 0;
        for (int i = 1; i < 7; i++)
        {
            value += MATERIAL_VALUE[i] * (board.GetPieceList((ChessChallenge.API.PieceType)i, true).Count - board.GetPieceList((ChessChallenge.API.PieceType)i, false).Count);
        }

        Move[] moves = board.GetLegalMoves();
        foreach (Move move in moves)
        {
            value += MOBILITY_VALUE[(int)move.MovePieceType];

            if (move.IsCastles)
            {
                value += CASTLE_VALUE;
            }
        }

        return value;
    }
}