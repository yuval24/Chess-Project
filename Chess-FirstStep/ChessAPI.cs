using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace Chess_FirstStep
{
    public class ChessAPI
    {
        private static readonly string StockfishApiBaseUrl = "https://stockfish.online/api/s/v2.php";

        public static async Task<string> GetBestMove(string fen)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string requestUrl = $"{StockfishApiBaseUrl}?fen={Uri.EscapeDataString(fen)}&depth={15}";

                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                // Extract the best move from the response
                var bestMove = json["bestmove"]?.ToString();
                return bestMove;
            }
        }

        public static string ConvertChessboardToFen(Chessboard chessboard, bool isWhiteToMove, string castlingRights, string enPassantTarget, int halfmoveClock, int fullmoveNumber)
        {
            StringBuilder resultFen = new StringBuilder();

            // Convert the board state to FEN string
            for (int i = 7; i >= 0; i--)
            {
                int emptyCount = 0;
                for (int j = 0; j < 8; j++)
                {
                    ChessPiece piece = chessboard.GetChessPieceAt(i, j);
                    if (piece == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0)
                        {
                            resultFen.Append(emptyCount);
                            emptyCount = 0;
                        }

                        char pieceChar = GetFenChar(piece);
                        resultFen.Append(pieceChar);
                    }
                }
                if (emptyCount > 0)
                {
                    resultFen.Append(emptyCount);
                }
                if (i > 0)
                {
                    resultFen.Append("/");
                }
            }

            // Add side to move
            resultFen.Append(isWhiteToMove ? " w " : " b ");

            // Add castling rights
            resultFen.Append(string.IsNullOrEmpty(castlingRights) ? "-" : castlingRights);
            resultFen.Append(" ");

            // Add en passant target square
            resultFen.Append(string.IsNullOrEmpty(enPassantTarget) ? "-" : enPassantTarget);
            resultFen.Append(" ");

            // Add halfmove clock
            resultFen.Append(halfmoveClock);
            resultFen.Append(" ");

            // Add fullmove number
            resultFen.Append(fullmoveNumber);

            return resultFen.ToString();
        }


        private static char GetFenChar(ChessPiece piece)
        {
            char pieceChar = ' ';
            switch (piece.Name)
            {
                case "King":
                    pieceChar = piece.IsWhite ? 'K' : 'k';
                    break;
                case "Queen":
                    pieceChar = piece.IsWhite ? 'Q' : 'q';
                    break;
                case "Rook":
                    pieceChar = piece.IsWhite ? 'R' : 'r';
                    break;
                case "Bishop":
                    pieceChar = piece.IsWhite ? 'B' : 'b';
                    break;
                case "Knight":
                    pieceChar = piece.IsWhite ? 'N' : 'n';
                    break;
                case "Pawn":
                    pieceChar = piece.IsWhite ? 'P' : 'p';
                    break;
            }
            return pieceChar;
        }
    }
}