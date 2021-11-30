using System;
using System.Collections.Generic;
using System.Collections;

public class WriteParser {

    public ConsoleColor default_foreground_colour = ConsoleColor.White;
    public ConsoleColor default_background_colour = ConsoleColor.Black;

    public WriteParser() {

    }






    public string getStringFrom(string input) {
        List<string> contents = new List<string>();
        foreach(SubString str in parseString(input)) {
            contents.Add(str.content);
        }
        return String.Join("", contents.ToArray());
    }

    public List<SubString> parseString(string inputt) {

        List<SubString> to_return = new List<SubString>();

        string input = inputt;

        if (!input.Contains('|')) {
            to_return.Add( new SubString {
                content = input,
                fg_colour = default_foreground_colour,
                bg_colour = default_background_colour,
            });
            return to_return;
        }

        if (countChar(input, '|') %2 != 0) {
            to_return.Add( new SubString {
                content = input,
                fg_colour = default_foreground_colour,
                bg_colour = default_background_colour,
            });
            return to_return;
        }

        if (input.Contains('|')) {
            to_return.Add( new SubString {
                content = input.Split('|', 2)[0],
                fg_colour = default_foreground_colour,
                bg_colour = default_background_colour,
            });
        }

        input = "|" + input.Split('|', 2)[1];

        while(input.Contains('|')) { // hello | ^ red| hi !

            /*to_return.Add( new SubString {
                    content = input.Split('|', 2)[0],
                    fg_colour = default_foreground_colour,
                    bg_colour = default_background_colour,
                });

            */

            //Console.WriteLine("input1 : " + input);

            string colour = input.Split('|', 2)[1].Split('|', 2)[0]; // red 

            //Console.WriteLine("colour: " +colour);

            (ConsoleColor fgcol, ConsoleColor bgcol) = getColoursFrom(colour);

            input = input.Split('|', 2)[1]; //input : hi !
            //Console.WriteLine("input: " + input);

            string content = input.Split('|', 2)[1]; // ?
            if (content.Contains('|')) {
                content = content.Split('|', 2)[0];
            }
            //Console.WriteLine("content2: " + content);
            
            input = input.Split('|', 2)[1]; // ?
            //Console.WriteLine("input2: " + input);
            

            to_return.Add(new SubString {
                content = content,
                fg_colour = fgcol,
                bg_colour = bgcol,
            });

        }

        return to_return;

    }

    private (ConsoleColor, ConsoleColor) getColoursFrom(string to_parse) {

        ConsoleColor fg_to_ret = default_foreground_colour;
        ConsoleColor bg_to_ret = default_background_colour;

        if (to_parse.Contains('/')) {
            string str1 = to_parse.Split('/',2)[0];
            string str2 = to_parse.Split('/',2)[1];

            fg_to_ret = colourFromString(str1);
            bg_to_ret = colourFromString(str2);
        }
        else {
            fg_to_ret = colourFromString(to_parse);
        }

        return (fg_to_ret, bg_to_ret);

    }

    private ConsoleColor colourFromString(string str) {

        str = str.ToLower();

        switch(str) {
            case "blue":
            case "bl":
                return ConsoleColor.Blue;
            
            case "red":
            case "r":
                return ConsoleColor.Red;
            
            case "green":
            case "gr":
                return ConsoleColor.Green;

            case "yellow":
                return ConsoleColor.Yellow;

            case "darkyellow":
                return ConsoleColor.DarkYellow;

            case "darkblue":
            case "dbl":
                return ConsoleColor.DarkBlue;

            case "darkred":
            case "dr":
                return ConsoleColor.DarkRed;

            case "darkgreen":
            case "dgr":
                return ConsoleColor.DarkGreen;

            case "magenta":
            case "mag":
                return ConsoleColor.Magenta;

            case "darkmagenta":
            case "dmag":
                return ConsoleColor.DarkMagenta;

            case "gray":
            case "grey":
                return ConsoleColor.Gray;

            case "darkgrey":
            case "darkgray":
                return ConsoleColor.DarkGray;

            case "cyan":
            case "cy":
                return ConsoleColor.Cyan;

            case "darkcyan":
            case "dcy":
                return ConsoleColor.DarkCyan;

            case "black":
                return ConsoleColor.Black;
            
            case "white":
            case "w":
                return ConsoleColor.White;

            case "def_fg":
            case "fg":
                return default_foreground_colour;
            
            case "def_bg":
            case "bg":
                return default_background_colour;

            default:
                return default_foreground_colour;
        }

    }

    private int countChar(string str, char c) {
        int count = 0;
        foreach (char ch in str) {
            if (ch == c) {
                count ++;
            }
        }
        return count;
    }

    public struct SubString {
        public string content;
        public ConsoleColor fg_colour;
        public ConsoleColor bg_colour;
    }

}