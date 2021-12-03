using System;
using System.Collections.Generic;
using System.Collections;

public class Input {


	public int xLimit = 0;

	public enum Format {
		center,
		left,
		right,
	}

	private char[] special_chars = new char[] {
		'/', '&', '#',
	};


	public Input() {
		xLimit = Console.WindowWidth - 2;
	}


	// return a string inputted by the user
	public string getString() {
		return Console.ReadLine();
	}

	// gets a string with a limit on how much the user can type as input
	public string getString(int limit) {

		if (limit < 1) {
			limit = 1;
		}

		string return_string = "";

		// int charsput = 0;
		int cursorYLoc = Console.CursorTop;

		ConsoleKeyInfo key;

		key = Console.ReadKey();

		while(key.Key != ConsoleKey.Enter) {
			if (key.Key == ConsoleKey.Backspace) {
				if (return_string.Length > 1) {
					return_string = return_string.Remove(return_string.Length - 1, 1);
				} else {
					return_string = "";
				}
				Console.Write(" ");
				Console.SetCursorPosition(Console.CursorLeft - 1, cursorYLoc);
				key = Console.ReadKey();
				continue;
			} 
			if ((Console.CursorLeft > limit)) {
				Console.Write(" ");
				Console.SetCursorPosition(Console.CursorLeft - 2, cursorYLoc);
				Console.Write(" ");
				Console.SetCursorPosition(Console.CursorLeft - 1, cursorYLoc);
			} else {
				if (char.IsLetter(key.KeyChar) || char.IsNumber(key.KeyChar) || containsSpecialChars(key.KeyChar)) {
					return_string += key.KeyChar;
				}
			}
			key = Console.ReadKey();
		}

		return return_string.ToLower();

	}

	private string padWhitespace(int count) {
		string ret = "";
		for(int i = 0; i < count; i ++) {
			ret += " ";
		}
		return ret;
	}

	private bool containsSpecialChars(char ch) {
		foreach (char c in special_chars) {
			if (c == ch) {
				return true;
			}
		}
		return false;
	}

	public void print(string s) {
		printToScr(s);
	}

	public void printnln(string s) {
		printToScr(s, false);
	}

	public void movex(int move) {
		if (Console.CursorLeft + move >= Console.WindowWidth) {
			move = Console.CursorLeft - move;
		}
		if (Console.CursorLeft + move <= 0) {
			move = Console.CursorLeft - move;
		}
		Console.SetCursorPosition(Console.CursorLeft + move, Console.CursorTop);
	}

	private void printToScr(string formattedString, bool newline = true, int xlim = 0) {
		if (string.IsNullOrEmpty(formattedString) && newline == false) {
			return;
		} else if (string.IsNullOrEmpty(formattedString)) {
			Console.WriteLine();
			return;
		}
		if (!formattedString.Contains("|")) {
			if (newline) {
				if (xlim != 0) {
				 	writeRaw(formattedString, xlim);
				} else {
				 	writeRaw(formattedString, xLimit);
				}
				Console.WriteLine();
			}
			else {
				if (xlim != 0) {
				 	writeRaw(formattedString, xlim);
				} else {
				 	writeRaw(formattedString, xLimit);
				}
			}
			return;
		}
		foreach(WriteParser.SubString str in WriteParser.parseString(formattedString)) {
			Console.ForegroundColor = str.fg_colour;
			Console.BackgroundColor = str.bg_colour;
			if (xlim != 0) {
			 	writeRaw(str.content, xlim);
			} else {
			 	writeRaw(str.content, xLimit);
			}
		}
		if (newline) Console.WriteLine();
	}

	private void printToScr(string formattedString,int x, int xlim = 0) {
		Console.SetCursorPosition(x, Console.CursorTop);
		if (string.IsNullOrEmpty(formattedString)) {
			Console.WriteLine();
			return;
		}
		if (!formattedString.Contains("|")) {
			
			if (xlim != 0) {
			 	writeRaw(formattedString, x, xlim);
			} else {
			 	writeRaw(formattedString, x, xLimit);
			}
			
			return;
		}
		foreach(WriteParser.SubString str in WriteParser.parseString(formattedString)) {
			if (Console.CursorLeft > (xlim == 0 ? xLimit : xlim)) {
				Console.WriteLine();
				Console.SetCursorPosition(x, Console.CursorTop);
			}
			Console.ForegroundColor = str.fg_colour;
			Console.BackgroundColor = str.bg_colour;
			if (xlim != 0) {
			 	writeRaw(str.content, x, xlim);
			} else {
			 	writeRaw(str.content, x, xLimit);
			}
		}
	}

	public void writeRaw(string str, int xlim) {
		if (xlim == 0) {
			xlim = xLimit;
		}
		foreach(char ch in str) {
			if (Console.CursorLeft > xlim - 2 || ch == '\n') {
				Console.WriteLine();
				continue;
			}
			Console.Write(ch);
		}
	}	

	public void writeRaw(string str, int x, int xlim) {
		if (xlim == 0) {
			xlim = xLimit;
		}
		foreach(char ch in str) {
			if (Console.CursorLeft > xlim - 2 || ch == '\n') {
				Console.SetCursorPosition(x, Console.CursorTop + 1);
				continue;
			}
			Console.Write(ch);
		}
	}		

	private int formattedLengthOf(string s) {
		int size = 0;
		if (!s.Contains("!")) {
			return s.Length;
		}
		foreach(WriteParser.SubString str in WriteParser.parseString(s)) {
			size += str.content.Length;
		}
		return size;
	}

	public void printf(string s, Format f) {
		switch (f) {
			case Format.left:
				printToScr(s);
				break;

			case Format.right:
				Console.SetCursorPosition(xLimit - formattedLengthOf(s), Console.CursorTop);
				printToScr(s);
				break;

			case Format.center:
				Console.SetCursorPosition(xLimit / 2 - formattedLengthOf(s), Console.CursorTop);
				printToScr(s);
				break;

			default:
				printToScr(s);
				break;
		}
	}

	public void drawBox(string content, char seperator = '█') {
		int oldY = Console.CursorTop;
		int xIndex = 2;

		Console.SetCursorPosition(2, oldY + 2);

		List<string> lines = new List<string>();
		string currentstring = "";

		foreach(char c in content) {
			if (xIndex > Console.WindowWidth - 4 || c == '\n') {
				lines.Add(currentstring);
				currentstring = "";
				xIndex = 2;
				continue;
			}
			xIndex ++; 
			currentstring += c;
		}
		lines.Add(currentstring);

		int lineindex = 0;

		foreach(string str in lines) {
			Console.SetCursorPosition(2, oldY + 2 + lineindex);
			printToScr(str, false);
			lineindex ++;
		}

		drawRect(0, oldY, formattedLengthOf(lines[0]) + 3, lines.Count + 4, seperator);
	}

	public void drawBox(string content, int x, int y, bool filled = false, char seperator = '█') {

		int oldY = y;
		int xIndex = x;

		Console.SetCursorPosition(x + 2, oldY + 2);

		string currentstring = "";

		string longestStr = "";
		int prevLongest = 0;
		int longest = 0;

		int lines = 1;

		foreach(char c in WriteParser.getStringFrom(content)) {
			longest ++;
			if (xIndex > Console.WindowWidth - 4 || c == '\n') {
				if (longest > prevLongest) {
					prevLongest = longest;
					longestStr = currentstring;
					longest = 0;
				}
				longest = 0;
				xIndex = x;
				lines ++;
				currentstring = "";
			}
			xIndex ++;
			currentstring += c;
		}
		if (longest > prevLongest) {
			prevLongest = longest;
			longestStr = currentstring;
			longest = 0;
		}
		lines ++;

		if (filled) {
			drawRectFilled(x, y, longestStr.Length + 2, lines + 4, seperator);
		}

		// foreach(string str in lines) {
		// 	Console.SetCursorPosition(x + 2, oldY + 2 + lineindex);
		// 	printToScr(str, false, x + formattedLengthOf(longestStr));
		// 	lineindex ++;
		// }

		printToScr(content, x + 2, x + longestStr.Length + 2);

		if (!filled) {
			drawRect(x, y, formattedLengthOf(longestStr) + 2, lines + 4, seperator);
		}
	}

	public void print(string s, int x, int y) {
		/*if (x > Console.WindowWidth - 1 || x < 1 || y > Console.WindowHeight - 1 || y < 0) {
			printToScr(s);
		} else {*/
			Console.SetCursorPosition(x, y);
			printToScr(s);
		// }
	}


	public void drawRect(int x, int y, int width, int height, char seperator = '█', ConsoleColor col = ConsoleColor.White) {
        ConsoleColor prev = Console.ForegroundColor;
        Console.ForegroundColor = col;
        Console.SetCursorPosition(x, y);
        // ceilings
        for (int i = x; i < x + width; i ++) {
            Console.Write(seperator);
        }
        Console.SetCursorPosition(x, y + (height - 1));
        for (int i = x; i < x + width; i ++) {
           Console.Write(seperator);
        }
        // walls
        Console.SetCursorPosition(x, y);
        for (int i = 0; i < height; i ++) {
            Console.Write(seperator);
            Console.SetCursorPosition(x, y + i);
        }
        Console.SetCursorPosition(x + width, y);
        for (int i = 0; i < height; i ++) {
            Console.Write(seperator);
            Console.SetCursorPosition(x + width, y + i);
        }
        Console.SetCursorPosition(x + width, y + height - 1);
        Console.Write(seperator);
        // Console.Write(seperator);
        Console.ForegroundColor = prev;
    }

    public void drawRectFilled(int x, int y, int width, int height, char seperator = '█') {
    	for (int i = x; i < x + width; i ++) {
    		for (int b = y; b < y + height; b ++) {
    			Console.SetCursorPosition(i,b);
    			Console.Write(" ");
    		}
    	}
    	for (int i = x; i < x + width; i ++) {
    		for (int b = y; b < y + height; b ++) {
    			Console.SetCursorPosition(i,b);
    			if (i == x && b == y) {
    				Console.Write(seperator);
    			}
    			if (i == x + width && b == y + height) {
    				Console.Write(seperator);
    			}
    			if (i < x + width - 1 && (b == y || b == y + height - 1)) {
    				Console.Write(seperator);
    			}
    			if (b < y + height && (i == x || i == x + width - 1)) {
    				Console.Write(seperator);
    			}
    		}
    	}
    }

    public void verticalLine(int x1, int y1, int y2, char seperator = '█') {
		for (int b = y1; b < y2; b ++) {
			Console.SetCursorPosition(x1, b);
			Console.Write(seperator);
		}
    }

}