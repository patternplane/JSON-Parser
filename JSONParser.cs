﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JSON_Parser
{
    enum state
    {
        obj,
        array,
        key,
        value_string,
        value_numeric,
        value_specialLetter
    }

    public class JSONParser
    {
        StringBuilder newStr = new StringBuilder();
        StringBuilder trashStr = new StringBuilder();
        bool getTrashValue;

        private void takeTrash()
        {
            if (!getTrashValue)
                return;

            byte[] bytes = Encoding.UTF8.GetBytes(trashStr.ToString());
            foreach (byte b in bytes)
                newStr.Append(b);
        }

        public char[] parse(char[] rawData, bool getTrashValue)
        {
            newStr.Clear();
            trashStr.Clear();
            this.getTrashValue = getTrashValue;
            Stack<state> stateS = new Stack<state>();
            int classStep = 0;
            bool escape = false;
            char lastchar = '\0';

            foreach (char c in rawData)
            {
                if (stateS.Count == 0)
                {
                    if (c == '{')
                    {
                        takeTrash();
                        if (newStr.Length > 0)
                            newStr.AppendLine();
                        for (int i = 0; i < classStep; i++)
                            newStr.Append('\t');
                        newStr.Append(c).AppendLine();
                        trashStr.Clear();
                        stateS.Push(state.obj);
                        classStep += 1;
                    }
                    else
                    {
                        trashStr.Append(c);
                    }
                }
                else 
                {
                    if (stateS.Peek() == state.obj)
                    {
                        if (c == '"')
                        {
                            takeTrash();
                            if (lastchar != ':')
                                for (int i = 0; i < classStep; i++)
                                    newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            if (lastchar == ':')
                                stateS.Push(state.value_string);
                            else
                            {
                                stateS.Push(state.key);
                                escape = false;
                            }
                        }
                        else if (c == ':' || c == ',')
                        {
                            takeTrash();
                            newStr.Append(c);
                            if (c == ',')
                                newStr.AppendLine();
                            trashStr.Clear();

                            lastchar = c;
                        }
                        else if (c == '[')
                        {
                            takeTrash();
                            if (lastchar != ':')
                                for (int i = 0; i < classStep; i++)
                                    newStr.Append('\t');
                            newStr.Append(c).AppendLine();
                            trashStr.Clear();

                            stateS.Push(state.array);
                            classStep += 1;
                        }
                        else if (c == '{')
                        {
                            takeTrash();
                            if (lastchar != ':')
                                for (int i = 0; i < classStep; i++)
                                    newStr.Append('\t');
                            newStr.Append(c).AppendLine();
                            trashStr.Clear();

                            stateS.Push(state.obj);
                            classStep += 1;
                            lastchar = '\0';
                        }
                        else if (c == '}')
                        {
                            takeTrash();
                            newStr.AppendLine();
                            for (int i = 0; i < classStep - 1; i++)
                                newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Pop();
                            classStep -= 1;
                            lastchar = '\0';
                        }
                        else if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                        {
                            takeTrash();
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Push(state.value_specialLetter);
                        }
                        else if (('0' <= c && c <= '9') || c == '.')
                        {
                            takeTrash();
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Push(state.value_numeric);
                        }
                        else
                        {
                            trashStr.Append(c);
                        }
                    }
                    else if (stateS.Peek() == state.array)
                    {
                        if (c == '"')
                        {
                            takeTrash();
                            for (int i = 0; i < classStep; i++)
                                newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Push(state.value_string);
                        }
                        else if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                        {
                            takeTrash();
                            for (int i = 0; i < classStep; i++)
                                newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Push(state.value_specialLetter);
                        }
                        else if (('0' <= c && c <= '9') || c == '.')
                        {
                            takeTrash();
                            for (int i = 0; i < classStep; i++)
                                newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Push(state.value_numeric);
                        }
                        else if (c == ',')
                        {
                            takeTrash();
                            newStr.Append(c).AppendLine();
                            trashStr.Clear();
                        }
                        else if (c == '{')
                        {
                            takeTrash();
                            for (int i = 0; i < classStep; i++)
                                newStr.Append('\t');
                            newStr.Append(c).AppendLine();
                            trashStr.Clear();

                            stateS.Push(state.obj);
                            classStep += 1;
                            lastchar = '\0';
                        }
                        else if (c == '[')
                        {
                            takeTrash();
                            for (int i = 0; i < classStep; i++)
                                newStr.Append('\t');
                            newStr.Append(c).AppendLine();
                            trashStr.Clear();

                            stateS.Push(state.array);
                            classStep += 1;
                        }
                        else if (c == ']')
                        {
                            takeTrash();
                            newStr.AppendLine();
                            for (int i = 0; i < classStep - 1; i++)
                                newStr.Append('\t');
                            newStr.Append(c);
                            trashStr.Clear();

                            stateS.Pop();
                            classStep -= 1;
                            lastchar = '\0';
                        }
                    }
                    else if (stateS.Peek() == state.key || stateS.Peek() == state.value_string)
                    {
                        newStr.Append(c);
                        if (escape)
                            escape = false;
                        else if (c == '\\')
                            escape = true;
                        else if (c == '"')
                        {
                            stateS.Pop();
                            lastchar = '\0';
                        }
                    }
                    else if (stateS.Peek() == state.value_numeric)
                    {
                        if (('0' <= c && c <= '9') || c == '.')
                            newStr.Append(c);
                        else if (c == '}' || c == ']' || c == ',')
                        {
                            if (c == '}' || c == ']')
                            {
                                newStr.AppendLine();
                                for (int i = 0; i < classStep -1; i++)
                                    newStr.Append('\t');
                            }
                            newStr.Append(c);
                            if (c == ',')
                                newStr.AppendLine();

                            stateS.Pop();
                            if (c != ',')
                            {
                                stateS.Pop();
                                classStep -= 1;
                            }
                            lastchar = '\0';
                        }
                        else
                        {
                            trashStr.Append(c);

                            stateS.Pop();
                            lastchar = '\0';
                        }
                    }
                    else if (stateS.Peek() == state.value_specialLetter)
                    {
                        if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                            newStr.Append(c);
                        else if (c == '}' || c == ']' || c == ',')
                        {
                            if (c == '}' || c == ']')
                            {
                                newStr.AppendLine();
                                for (int i = 0; i < classStep -1 ; i++)
                                    newStr.Append('\t');
                            }
                            newStr.Append(c);
                            if (c == ',')
                                newStr.AppendLine();

                            stateS.Pop();
                            if (c != ',')
                            {
                                stateS.Pop();
                                classStep -= 1;
                            }
                            lastchar = '\0';
                        }
                        else
                        {
                            trashStr.Append(c);

                            stateS.Pop();
                            lastchar = '\0';
                        }
                    }
                }
            }
            takeTrash();

            char[] result = new char[newStr.Length];
            newStr.CopyTo(0, result, 0, newStr.Length);
            return result;
        }
    }
}
