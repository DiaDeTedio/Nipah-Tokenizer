using System;
using System.Reflection;
using System.Collections.Generic;

namespace NipahTokenizer
{
    public class ParserElement
	{
		public static readonly EmptyElement Empty = new EmptyElement();
		public ParserElement Parent => parent;
		public ParserElement Child => child;
		public ParserElement Conditional => conditional;
		ParserElement parent;
		ParserElement conditional = Empty;
		ParserElement child;
		static ParserValues values;
		public static void BeginValues(ParserValues _values)
		{
			values = _values;
		}
		public static void EndValues() => values = null;
		public static ParserValues GetValues() => values;

		public ParserElement Set(string name, object value)
		{
			values.Set(name, value);
			return this;
		}
		public ParserElement Get(string name, out object value)
		{
			values.GetOut(name, out value);
			return this;
		}
		public object Get(string name)
		{
			return values.Get(name);
		}

		public ParserElement Then(string text) => Token(text);
		public ParserElement Then(TokenType type) => Token(type);
		public ParserElement Then(TokenType type, string storeOn) => Token(type, storeOn);
		public ParserElement ThenValue() => TokenValue();
		public ParserElement ThenValue(string storeOn) => TokenValue(storeOn);
		public ParserElement ThenAny() => TokenAny();
		public ParserElement ThenAny(string storeOn) => TokenAny(storeOn);

		public ParserElement Token(string text) 
		=> child = new TokenElement(TokenConditional.Text).Config(text, TokenType.None, null);
		public ParserElement Token(TokenType type) 
		=> child = new TokenElement(TokenConditional.Type).Config(null, type, null);
		public ParserElement Token(TokenType type, string storeOn) 
		=> child = new TokenElement(TokenConditional.Type).Config(null, type, storeOn);
		public ParserElement TokenValue()
		=> child = new TokenElement(TokenConditional.Value).Config(null, default(TokenType), null);
		public ParserElement TokenValue(string storeOn)
		=> child = new TokenElement(TokenConditional.Value).Config(null, default(TokenType), storeOn);
		public ParserElement TokenAny()
		=> child = new TokenElement(TokenConditional.Any).Config(null, default(TokenType), null);
		public ParserElement TokenAny(string storeOn)
		=> child = new TokenElement(TokenConditional.Any).Config(null, default(TokenType), storeOn);

		public ParserElement End() => Token(TokenType.EOF);

		public ParserElement Try(ParserElement conditional, Func<ParserElement> onWork)
		=> child = new TryElement(conditional, onWork);

		T create<T>() where T : ParserElement, new()
		{
			child = new T();
			return (T)child;
		}

		public ParserElement Setup(ParserElement parent, ParserElement conditional = null)
		{
			this.parent = parent;
			if(conditional != null)
				this.conditional = conditional;
			return this;
		}



		public virtual bool Match(ProgressiveList<Token> tokens)
		{
			return false;
		}
		public virtual void Work(ProgressiveList<Token> tokens, ParserResult result)
		{

		}

		protected void DefWork(ProgressiveList<Token> tokens, ParserResult result)
		{
			if(child?.Match(tokens) ?? false)
				child?.Work(tokens, result);
		}
	}
	public class EmptyElement : ParserElement
	{
		public override bool Match(ProgressiveList<Token> tokens)
		{
			return true;
		}
	}
	public class ParserValues
	{
		Dictionary<string, object> values = new Dictionary<string, object>(32);

		public object Get(string name)
		{
			object value;
			values.TryGetValue(name, out value);
			return value;
		}
		public bool GetOut(string name, out object value) 
		=> values.TryGetValue(name, out value);
		public void Set(string name, object value) => values[name] = value;

		public void Clear()
		{
			values.Clear();
		}
	}
	public class MainParser : ParserElement
	{
		Func<ParserValues, dynamic> getResult;
		public MainParser Setup(ParserElement parent, ParserElement conditional,
		                  Func<ParserValues, dynamic> getResult)
		{
			Setup(parent, conditional);

			this.getResult = getResult;

			return this;
		}
		public dynamic GetResult() => getResult(ParserElement.GetValues());
		public override bool Match(ProgressiveList<Token> tokens)
		{
			return Conditional.Match(tokens);
		}
		public override void Work(ProgressiveList<Token> tokens, ParserResult result)
		{
			if(Child.Match(tokens))
				Child.Work(tokens, result);
			else
				tokens.This().Or(tokens.Look_Back()).Error();
		}
	}
}
