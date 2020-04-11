using System;
using System.Reflection;
using System.Collections.Generic;

namespace NipahTokenizer
{
    public class Parser : ParserElement
	{
		Dictionary<string, ParserElement> parsers = new Dictionary<string, ParserElement>(32);

		public ParserElement New(string id, Func<ParserValues, dynamic> getResult = null,
		                         ParserElement conditional = null)
		{
			return parsers[id] = new MainParser().Setup(this, conditional, getResult);
		}

		public ParserResult Parse(ProgressiveList<Token> tokens)
		{
			var result = new ParserResult();

			var values = new ParserValues();

			BeginValues(values);
			begin :
			foreach(var p in parsers)
			{
				var parser = (MainParser)p.Value;
				int state = tokens.GetState();
				if(parser.Match(tokens))
				{
					parser.Work(tokens, result);
					result.Include(parser.GetResult());
					goto begin;
				}
				else
					tokens.RestoreState(state);
				values.Clear();
			}
			EndValues();

			return result;
		}
	}
	public class ParserResult
	{
		List<dynamic> result = new List<dynamic>(32);

		public void Include(dynamic res) => result.Add(res);

		public override string ToString()
		{
			string str = "";
			foreach(var value in result)
			{
				str += value.ToString() + '\n';
			}
			return str;
		}
	}
}
