using System;
using System.Reflection;
using System.Collections.Generic;

namespace NipahTokenizer
{
    public class TokenElement : ParserElement
	{
		string text;
		TokenType type;
		string storeOn;
		TokenConditional condType;

		public TokenElement Config(string text, TokenType type, string storeOn)
		{
			if(condType == TokenConditional.Text)
				this.text = text;
			else if(condType == TokenConditional.Type)
				this.type = type;

			this.storeOn = storeOn;
			return this;
		}

		public override bool Match(ProgressiveList<Token> tokens)
		{
			Token token;
			bool result;
			switch(condType)
			{
				case TokenConditional.Text:
					return tokens.Next().text == text;
				case TokenConditional.Type:
					token = tokens.Next();
					result = token.Type() == type;
					if(result && storeOn != null)
						Set(storeOn, token.value);
					return result;
				case TokenConditional.Value:
					token = tokens.Next();
					result = Tokenizer.isValue(token.Type());
					if(result && storeOn != null)
						Set(storeOn, token.value);
					return result;
				case TokenConditional.Any:
					token = tokens.Next();
					result = true;
					if(storeOn != null)
						Set(storeOn, token.value);
					return result;
			}
			return false;
		}
		public override void Work(ProgressiveList<Token> tokens, ParserResult result)
		{
			DefWork(tokens, result);
		}

		public TokenElement(TokenConditional of)
		{
			condType = of;
		}
	}
	public enum TokenConditional
	{
		Text,
		Type,
		Value,
		Any
	}

	public class TryElement : ParserElement
	{
		Func<ParserElement> onWork;

		public override bool Match(ProgressiveList<Token> tokens)
		{
			return true;
		}
		public override void Work(ProgressiveList<Token> tokens, ParserResult result)
		{
			if(Conditional.Match(tokens))
			{
				var work = onWork();
				if(work != null)
				{
					if(work.Match(tokens))
						work.Work(tokens, result);
				}
			}
			Child?.Work(tokens, result);
		}

		public TryElement(ParserElement conditional, Func<ParserElement> onWork)
		{
			Setup(null, conditional);
			this.onWork = onWork;
		}
	}
}
