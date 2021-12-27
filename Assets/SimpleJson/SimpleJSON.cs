/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Changelog now external. See Changelog.txt
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2019 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.ComponentModel;        
using System.Reflection;
namespace SimpleJSON{
    public enum JSONNodeType{
        Array = 1,
        Object = 2,
        String = 3,
        Number = 4,
        NullValue = 5,
        Boolean = 6,
        None = 7,
        Custom = 0xFF,
    }
    public enum JSONTextMode{
        Compact,
        Indent
    }

    public abstract partial class JSONNode{

        #region Enumerators
        public struct Enumerator{
            private enum Type { 
                None, 
                Array, 
                Object 
            }
            private Type type;
            private Dictionary<string, JSONNode>.Enumerator m_Object;
            private List<JSONNode>.Enumerator m_Array;
            public bool IsValid { 
                get { 
                    return type != Type.None; 
                } 
            }
            public Enumerator(List<JSONNode>.Enumerator arrayEnum_){
                type = Type.Array;
                m_Object = default(Dictionary<string, JSONNode>.Enumerator);
                m_Array = arrayEnum_;
            }
            public Enumerator(Dictionary<string, JSONNode>.Enumerator dictEnum_){
                type = Type.Object;
                m_Object = dictEnum_;
                m_Array = default(List<JSONNode>.Enumerator);
            }
            public KeyValuePair<string, JSONNode> Current{
                get{
                    if (type == Type.Array){
                        return new KeyValuePair<string, JSONNode>(string.Empty, m_Array.Current);
                    }else if (type == Type.Object){
                        return m_Object.Current;
                    }
                    return new KeyValuePair<string, JSONNode>(string.Empty, null);
                }
            }
            public bool MoveNext(){
                if (type == Type.Array){
                    return m_Array.MoveNext();
                }else if (type == Type.Object){
                    return m_Object.MoveNext();
                }    
                return false;
            }
        }
        public struct ValueEnumerator{
            private Enumerator m_Enumerator;
            public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { 

            }
            public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { 

            }
            public ValueEnumerator(Enumerator aEnumerator) { 
                m_Enumerator = aEnumerator; 
            }
            public JSONNode Current { 
                get {
                    return m_Enumerator.Current.Value; 
                } 
            }
            public bool MoveNext() { 
                return m_Enumerator.MoveNext(); 
            }
            public ValueEnumerator GetEnumerator() { 
                return this; 
            }
        }
        public struct KeyEnumerator{
            private Enumerator m_Enumerator;
            public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { 

            }
            public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { 

            }
            public KeyEnumerator(Enumerator aEnumerator) { 
                m_Enumerator = aEnumerator; 
            }
            public string Current { 
                get { 
                    return m_Enumerator.Current.Key; 
                } 
            }
            public bool MoveNext() { 
                return m_Enumerator.MoveNext(); 
            }
            public KeyEnumerator GetEnumerator() { 
                return this; 
            }
        }

        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerable<KeyValuePair<string, JSONNode>>{
            private JSONNode m_Node;
            private Enumerator m_Enumerator;
            internal LinqEnumerator(JSONNode aNode){
                m_Node = aNode;
                if (m_Node != null){
                    m_Enumerator = m_Node.GetEnumerator();
                }
            }
            public KeyValuePair<string, JSONNode> Current { 
                get {
                    return m_Enumerator.Current; 
                } 
            }
            object IEnumerator.Current { 
                get { 
                    return m_Enumerator.Current; 
                } 
            }
            public bool MoveNext() { 
                return m_Enumerator.MoveNext(); 
            }

            public void Dispose(){
                m_Node = null;
                m_Enumerator = new Enumerator();
            }

            public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator(){
                return new LinqEnumerator(m_Node);
            }

            public void Reset(){
                if (m_Node != null){
                    m_Enumerator = m_Node.GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator(){
                return new LinqEnumerator(m_Node);
            }
        }

        #endregion Enumerators

        #region common interface

        public static bool forceASCII = false; // Use Unicode by default
        public static bool longAsString = false; // lazy creator creates a JSONString instead of JSONNumber
        public static bool allowLineComments = true; // allow "//"-style comments at the end of a line

        public abstract JSONNodeType Tag { get; }

        public virtual JSONNode this[int aIndex] { 
            get { 
                return null; 
            } 
            set { 

            } 
        }

        public virtual JSONNode this[string aKey] { 
            get { 
                return null; 
            } 
            set {

            }
        }

        public virtual string Value {
            get {
                return ""; 
            } 
            set {

            } 
        }

        public virtual int Count {
            get {
                return 0;
            } 
        }

        public virtual bool IsNumber {
            get {
                return false;
            }
        }
        public virtual bool IsString {
            get {
                return false;
            }
        }
        public virtual bool IsBoolean {
            get {
                return false;
            }
        }
        public virtual bool IsNull {
            get {
                return false;
            }
        }
        public virtual bool IsArray {
            get {
                return false;
            }
        }
        public virtual bool IsObject {
            get {
                return false;
            }
        }

        public virtual bool Inline {
            get {
                return false;
            }
            set {

            }
        }

        public virtual void AddKeyValue(string key_, JSONNode jsonNodeAsItem_){
            
        }
        public virtual void AddItem(JSONNode jsonNodeAsItem_){
            AddKeyValue("", jsonNodeAsItem_);
        }

        public virtual JSONNode Remove(string key_){
            return null;
        }

        public virtual JSONNode Remove(int idx_){
            return null;
        }

        public virtual JSONNode Remove(JSONNode jsonNode_){
            return jsonNode_;
        }
        public virtual void Clear() { 

        }

        public virtual JSONNode Clone(){
            return null;
        }

        public virtual IEnumerable<JSONNode> Children{
            get{
                yield break;
            }
        }

        public IEnumerable<JSONNode> DeepChildren{
            get{
                foreach (var _children in Children){
                    foreach (var _deepChildren in _children.DeepChildren){
                        yield return _deepChildren;
                    }
                }
            }
        }

        public virtual bool HasKey(string key_){
            return false;
        }

        public virtual JSONNode GetValueOrDefault(string key_, JSONNode default_){
            return default_;
        }

        public override string ToString(){
            StringBuilder _sb = new StringBuilder();
            WriteToStringBuilder(_sb, 0, 0, JSONTextMode.Compact);
            return _sb.ToString();
        }

        public virtual string ToString(int indent_){
            StringBuilder _sb = new StringBuilder();
            WriteToStringBuilder(_sb, 0, indent_, JSONTextMode.Indent);
            return _sb.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_);

        public abstract Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JSONNode>> Linq { 
            get { 
                return new LinqEnumerator(this); 
            } 
        }
        public KeyEnumerator Keys { 
            get { 
                return new KeyEnumerator(GetEnumerator()); 
            } 
        }
        public ValueEnumerator Values { 
            get { 
                return new ValueEnumerator(GetEnumerator()); 
            } 
        }

        #endregion common interface

        #region typecasting properties


        public virtual double AsDouble{
            get{
                double _v = 0.0;
                if (double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out _v)){
                    return _v;
                }
                return 0.0;
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Value = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public virtual int AsInt{
            get {
                return (int)AsDouble; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                AsDouble = value; 
            }
        }

        public virtual float AsFloat{
            get {
                return (float)AsDouble; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                AsDouble = value; 
            }
        }

        public virtual bool AsBool{
            get{
                bool _v = false;
                if (bool.TryParse(Value, out _v)){
                    return _v;
                }
                return !string.IsNullOrEmpty(Value);
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                if(value){
                   Value = "true";
                }else{
                    Value = "false";
                }
            }
        }

        public virtual long AsLong{
            get{
                long _val = 0;
                if (long.TryParse(Value, out _val)){
                    return _val;
                }
                return 0L;
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Value = value.ToString();
            }
        }

        public virtual ulong AsULong{
            get{
                ulong _val = 0;
                if (ulong.TryParse(Value, out _val)){
                    return _val;
                }
                return 0;
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Value = value.ToString();
            }
        }

        public virtual JSONArray AsArray{
            get{
                return this as JSONArray;
            }
        }

        public virtual JSONObject AsObject{
            get{
                return this as JSONObject;
            }
        }


        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string str_){
            return (str_ == null) ? (JSONNode) JSONNull.CreateOrGet() : new JSONString(str_);
        }
        public static implicit operator string(JSONNode strJsNode_){
            return (strJsNode_ == null) ? null : strJsNode_.Value;
        }

        public static implicit operator JSONNode(double double_){
            return new JSONNumber(double_);
        }
        public static implicit operator double(JSONNode doubleJsNode_){
            return (doubleJsNode_ == null) ? 0 : doubleJsNode_.AsDouble;
        }

        public static implicit operator JSONNode(float float_){
            return new JSONNumber(float_);
        }
        public static implicit operator float(JSONNode floatJsNode_){
            return (floatJsNode_ == null) ? 0 : floatJsNode_.AsFloat;
        }

        public static implicit operator JSONNode(int int_){
            return new JSONNumber(int_);
        }
        public static implicit operator int(JSONNode intJsNode_){
            return (intJsNode_ == null) ? 0 : intJsNode_.AsInt;
        }

        public static implicit operator JSONNode(long long_){
            if (longAsString){
                return new JSONString(long_.ToString());
            }
            return new JSONNumber(long_);
        }
        public static implicit operator long(JSONNode longJsNode_){
            return (longJsNode_ == null) ? 0L : longJsNode_.AsLong;
        }

        public static implicit operator JSONNode(ulong ulong_){
            if (longAsString){
                return new JSONString(ulong_.ToString());
            }
            return new JSONNumber(ulong_);
        }
        public static implicit operator ulong(JSONNode ulongJsNode_){
            return (ulongJsNode_ == null) ? 0 : ulongJsNode_.AsULong;
        }

        public static implicit operator JSONNode(bool bool_){
            return new JSONBool(bool_);
        }
        public static implicit operator bool(JSONNode boolJsNode_){
            return (boolJsNode_ == null) ? false : boolJsNode_.AsBool;
        }

        public static implicit operator JSONNode(KeyValuePair<string, JSONNode> keyValuePair_){
            return keyValuePair_.Value;
        }

        public static bool operator ==(JSONNode jsonNode_, object value_){
            if (ReferenceEquals(jsonNode_, value_)){
                return true;
            }
            bool _aIsNull = jsonNode_ is JSONNull || ReferenceEquals(jsonNode_, null);
            bool _bIsNull = value_ is JSONNull || ReferenceEquals(value_, null);
            if (_aIsNull && _bIsNull){
                return true;
            }
            return !_aIsNull && jsonNode_.Equals(value_);
        }

        public static bool operator !=(JSONNode jsonNode_, object value_){
            return !(jsonNode_ == value_);
        }

        public override bool Equals(object obj_){
            return ReferenceEquals(this, obj_);
        }

        public override int GetHashCode(){
            return base.GetHashCode();
        }

        #endregion operators

        [ThreadStatic]
        private static StringBuilder m_EscapeBuilder;
        internal static StringBuilder EscapeBuilder{
            get{
                if (m_EscapeBuilder == null){
                    m_EscapeBuilder = new StringBuilder();
                }
                return m_EscapeBuilder;
            }
        }
        internal static string Escape(string str_){
            var _sb = EscapeBuilder;
            _sb.Length = 0;
            if (_sb.Capacity < str_.Length + str_.Length / 10){
                _sb.Capacity = str_.Length + str_.Length / 10;
            }
            foreach (char _char in str_){
                switch (_char){
                    case '\\':
                        _sb.Append("\\\\");
                        break;
                    case '\"':
                        _sb.Append("\\\"");
                        break;
                    case '\n':
                        _sb.Append("\\n");
                        break;
                    case '\r':
                        _sb.Append("\\r");
                        break;
                    case '\t':
                        _sb.Append("\\t");
                        break;
                    case '\b':
                        _sb.Append("\\b");
                        break;
                    case '\f':
                        _sb.Append("\\f");
                        break;
                    default:
                        if (_char < ' ' || (forceASCII && _char > 127)){
                            ushort _val = _char;
                            _sb.Append("\\u").Append(_val.ToString("X4"));
                        }else{
                            _sb.Append(_char);
                        }
                        break;
                }
            }
            string _result = _sb.ToString();
            _sb.Length = 0;
            return _result;
        }

        private static JSONNode ParseElement(string token_, bool quoted_){
            if (quoted_){
                return token_;
            }
            if (token_.Length <= 5){
                string _tmp = token_.ToLower();
                if (_tmp == "false" || _tmp == "true"){
                    return _tmp == "true";
                }
                if (_tmp == "null"){
                    return JSONNull.CreateOrGet();
                }
                    
            }
            double _val;
            if (double.TryParse(token_, NumberStyles.Float, CultureInfo.InvariantCulture, out _val)){
                return _val;
            }else{
                return token_;
            }
        }
        public static JSONNode Parse(string jsonStr_)
        {
            Stack<JSONNode> _stack = new Stack<JSONNode>();
            JSONNode _ctxJsNode = null;
            int _charIdx = 0;
            StringBuilder _sb = new StringBuilder();
            string _keyName = "";
            bool _isQuoteMode = false;
            bool _isKeyInQuoted = false;
            bool _hasNewlineChar = false;
            while (_charIdx < jsonStr_.Length){
                switch (jsonStr_[_charIdx]){
                    case '{':
                        if (_isQuoteMode){
                            _sb.Append(jsonStr_[_charIdx]);
                            break;
                        }
                        _stack.Push(new JSONObject());
                        if (_ctxJsNode != null){
                            _ctxJsNode.AddKeyValue(_keyName, _stack.Peek());
                        }
                        _keyName = "";
                        _sb.Length = 0;
                        _ctxJsNode = _stack.Peek();
                        _hasNewlineChar = false;
                        break;

                    case '[':
                        if (_isQuoteMode){
                            _sb.Append(jsonStr_[_charIdx]);
                            break;
                        }

                        _stack.Push(new JSONArray());
                        if (_ctxJsNode != null){
                            _ctxJsNode.AddKeyValue(_keyName, _stack.Peek());
                        }
                        _keyName = "";
                        _sb.Length = 0;
                        _ctxJsNode = _stack.Peek();
                        _hasNewlineChar = false;
                        break;

                    case '}':
                    case ']':
                        if (_isQuoteMode){
                            _sb.Append(jsonStr_[_charIdx]);
                            break;
                        }
                        if (_stack.Count == 0){
                            throw new Exception("JSON Parse: Too many closing brackets");
                        }

                        _stack.Pop();
                        if (_sb.Length > 0 || _isKeyInQuoted){
                            _ctxJsNode.AddKeyValue(_keyName, ParseElement(_sb.ToString(), _isKeyInQuoted));
                        }   
                        if (_ctxJsNode != null){
                            _ctxJsNode.Inline = !_hasNewlineChar;
                        }
                        _isKeyInQuoted = false;
                        _keyName = "";
                        _sb.Length = 0;
                        if (_stack.Count > 0){
                            _ctxJsNode = _stack.Peek();
                        }
                        break;

                    case ':':
                        if (_isQuoteMode){
                            _sb.Append(jsonStr_[_charIdx]);
                            break;
                        }
                        _keyName = _sb.ToString();
                        _sb.Length = 0;
                        _isKeyInQuoted = false;
                        break;

                    case '"':
                        _isQuoteMode ^= true;
                        _isKeyInQuoted |= _isQuoteMode;
                        break;

                    case ',':
                        if (_isQuoteMode){
                            _sb.Append(jsonStr_[_charIdx]);
                            break;
                        }
                        if (_sb.Length > 0 || _isKeyInQuoted){
                            _ctxJsNode.AddKeyValue(_keyName, ParseElement(_sb.ToString(), _isKeyInQuoted));
                        }
                        _isKeyInQuoted = false;
                        _keyName = "";
                        _sb.Length = 0;
                        _isKeyInQuoted = false;
                        break;

                    case '\r':
                    case '\n':
                        _hasNewlineChar = true;
                        break;

                    case ' ':
                    case '\t':
                        if (_isQuoteMode)
                            _sb.Append(jsonStr_[_charIdx]);
                        break;

                    case '\\':
                        ++_charIdx;
                        if (_isQuoteMode){
                            char _char = jsonStr_[_charIdx];
                            switch (_char){
                                case 't':
                                    _sb.Append('\t');
                                    break;
                                case 'r':
                                    _sb.Append('\r');
                                    break;
                                case 'n':
                                    _sb.Append('\n');
                                    break;
                                case 'b':
                                    _sb.Append('\b');
                                    break;
                                case 'f':
                                    _sb.Append('\f');
                                    break;
                                case 'u':
                                    {
                                        string _tempStr = jsonStr_.Substring(_charIdx + 1, 4);
                                        _sb.Append((char)int.Parse(
                                            _tempStr,
                                            System.Globalization.NumberStyles.AllowHexSpecifier));
                                        _charIdx += 4;
                                        break;
                                    }
                                default:
                                    _sb.Append(_char);
                                    break;
                            }
                        }
                        break;
                    case '/':
                        if (allowLineComments && !_isQuoteMode && _charIdx + 1 < jsonStr_.Length && jsonStr_[_charIdx + 1] == '/')
                        {
                            while (++_charIdx < jsonStr_.Length && jsonStr_[_charIdx] != '\n' && jsonStr_[_charIdx] != '\r') ;
                            break;
                        }
                        _sb.Append(jsonStr_[_charIdx]);
                        break;
                    case '\uFEFF': // remove / ignore BOM (Byte Order Mark)
                        break;
                    default:
                        _sb.Append(jsonStr_[_charIdx]);
                        break;
                }
                ++_charIdx;
            }
            if (_isQuoteMode){
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            if (_ctxJsNode == null){
                return ParseElement(_sb.ToString(), _isKeyInQuoted);
            }    
            return _ctxJsNode;
        }
    }
    // End of JSONNode

    public partial class JSONArray : JSONNode{
        private List<JSONNode> m_List = new List<JSONNode>();
        private bool inline = false;

        public JSONArray(){
            
        }

        public override bool Inline{
            get { 
                return inline; 
            }
            set { 
                inline = value; 
            }
        }

        public override JSONNodeType Tag { 
            get { 
                return JSONNodeType.Array; 
            }
        }
        public override bool IsArray { 
            get { 
                return true; 
            } 
        }

        public override Enumerator GetEnumerator() { 
            return new Enumerator(m_List.GetEnumerator()); 
        }

        public override JSONNode this[int idx_]{
            get{
                if (idx_ < 0 || idx_ >= m_List.Count){
                    return null;
                }
                return m_List[idx_];
            }
            set{
                bool _needChange = false;
                JSONNode _jsNode = value;
                if (_jsNode == null){
                    _jsNode = JSONNull.CreateOrGet();
                }else if(_jsNode.dataPath != null){
                    _jsNode = _jsNode.Clone();
                }
                if (idx_ < 0 || idx_ > m_List.Count){
                    throw new Exception("ERROR : " + dataPath + " 无法在序位 " + idx_.toIdxKey() + " 上添加元素。当前数组长度为 : " +  m_List.Count.ToString());
                }else if(idx_ == m_List.Count){//追加
                    m_List.Add(_jsNode);
                    _needChange = true;
                    arrayLength.AsInt = m_List.Count;
                    if(root != null){
                        root.changeValue(dataPath,this);
                        root.changeValue(arrayLength.dataPath,arrayLength);
                    }
                }else{//修改当前内容
                    JSONNode _currentJsonNode = m_List[idx_];
                    if(_currentJsonNode != value){
                        _currentJsonNode.dataPath = null;//当前内容要重置
                        m_List[idx_] = _jsNode;
                        _needChange = true;
                    }
                    _currentJsonNode = null;
                }
                if(_needChange && dataPath != null){
                    _jsNode.root = root;
                    _jsNode.dataPath = joinPath(dataPath,idx_.toIdxKey());
                }
            }
        }

        public override JSONNode this[string key_]{
            get {
                if(key_.IndexOf('.')>0){
                    return getJsonNodeByRelativePath(key_);
                }
                if(key_ == "length"){
                    return arrayLength;
                }else if(key_.isIdxKey()){
                    return this[key_.asKeyToIdx()];
                }else{
                    throw new Exception("ERROR : " + dataPath.ToString() + " 作为数组，只有length键。" );
                }
            }
            set{
                if(key_ == null){
                    throw new Exception("ERROR : 键 为 空");
                }
                if(key_.IndexOf('.')>0){
                    setToRelativePath(key_,value);
                }else{
                    if(key_.isIdxKey()){
                        int _idx = key_.asKeyToIdx();
                        JSONNode _currentJsonNode = this[_idx];
                        if(_currentJsonNode != null){
                            _currentJsonNode.dataPath = null;
                        }
                        this[_idx] = value;
                    }else{
                        throw new Exception("ERROR : " + dataPath.ToString() + " 是一个数组，无法添加键值对儿。" );
                    }   
                }
            }
        }

        public override int Count{
            get { 
                return m_List.Count; 
            }
        }

        public override void AddKeyValue(string key_, JSONNode itemJsonNode_){
            if (itemJsonNode_ == null){
                itemJsonNode_ = JSONNull.CreateOrGet();
            }
            this[m_List.Count] = itemJsonNode_;
            arrayLength.AsInt = m_List.Count;
        }
        public JSONNode RemoveAt(int idx_){
            return Remove(idx_);
        }
        public override JSONNode Remove(int idx_){
            if (idx_ < 0 || idx_ >= m_List.Count){
                return null;
            }
            JSONNode _itemJsonNode = m_List[idx_];
            if(dataPath != null){
                _itemJsonNode.dataPath = null;
                resetItemIdxAfter(idx_, -1);//之后的都要减少一。
                m_List.RemoveAt(idx_);
                arrayLength.AsInt = m_List.Count;
                root.changeValue(dataPath,this);
                root.changeValue(arrayLength.dataPath,arrayLength);
            }else{
                m_List.RemoveAt(idx_);
            }
            return _itemJsonNode;
        }

        public override JSONNode Remove(JSONNode jsonNode_){
            int _idx = m_List.IndexOf(jsonNode_);
            if(_idx < 0){
                return null;
            }
            if(dataPath != null){
                resetItemIdxAfter(_idx, -1);//之后的都要减少一。
                jsonNode_.dataPath = null;
                m_List.Remove(jsonNode_);
                arrayLength.AsInt = m_List.Count;
                root.changeValue(dataPath,this);
                root.changeValue(arrayLength.dataPath,arrayLength);
            }else{
                m_List.Remove(jsonNode_);
            }
            return jsonNode_;
        }

        public override void Clear(){
            arrayLength.AsInt = 0;
            if(dataPath == null){
                foreach(var _item in m_List){
                    if (_item != null){
                        _item.Clear();
                    }
                }
            }else{
                foreach(var _item in m_List){
                    if (_item != null){
                        _item.dataPath = null;
                        _item.Clear();
                    }
                }
                root.changeValue(dataPath,this);
                root.changeValue(arrayLength.dataPath,arrayLength);
            }
            m_List.Clear();
        }

        public override JSONNode Clone(){
            var _jsonArray = new JSONArray();
            _jsonArray.m_List.Capacity = m_List.Capacity;
            foreach(var _item in m_List){
                if (_item != null){
                    _jsonArray.AddItem(_item.Clone());
                }else{
                    _jsonArray.AddItem(null);
                }
            }
            _jsonArray.arrayLength = arrayLength.Clone() as JSONNumber;
            return _jsonArray;
        }

        public override IEnumerable<JSONNode> Children{
            get{
                foreach (JSONNode _jsNode in m_List){
                    yield return _jsNode;
                }
            }
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int aIndent, int aIndentInc, JSONTextMode aMode){
            sb_.Append('[');
            int count = m_List.Count;
            if (inline){
                aMode = JSONTextMode.Compact;
            }
            for (int _idx = 0; _idx < count; _idx++){
                if (_idx > 0){
                    sb_.Append(',');
                }
                if (aMode == JSONTextMode.Indent){
                    sb_.AppendLine();
                }
                if (aMode == JSONTextMode.Indent){
                    sb_.Append(' ', aIndent + aIndentInc);
                }
                m_List[_idx].WriteToStringBuilder(sb_, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JSONTextMode.Indent){
                sb_.AppendLine().Append(' ', aIndent);
            }
            sb_.Append(']');
        }
    }
    // End of JSONArray

    public partial class JSONObject : JSONNode{
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        private bool inline = false;

        public JSONObject(){
            
        }

        public override bool Inline{
            get { 
                return inline; 
            }
            set { 
                inline = value;
            }
        }

        public override JSONNodeType Tag { 
            get { 
                return JSONNodeType.Object; 
            } 
        }
        public override bool IsObject { 
            get { 
                return true; 
            } 
        }

        public override Enumerator GetEnumerator() { 
            return new Enumerator(m_Dict.GetEnumerator());
        }

        public override JSONNode this[string key_]{
            get{
                if(key_.IndexOf('.')>0){
                    return getJsonNodeByRelativePath(key_);
                }
                JSONNode _jsNodeForKey = null;
                if(m_Dict.TryGetValue(key_, out _jsNodeForKey)){
                    return _jsNodeForKey;
                }
                return null;
            }
            set{
                if(key_ == null){
                    if(dataPath == null){
                        throw new Exception("ERROR : 取值时，键为空");
                    }else{
                        throw new Exception("ERROR : " +dataPath.ToString() + "取值时，键为空");
                    }
                }
                if(key_.isIdxKey()){
                    if(dataPath == null){
                        throw new Exception("ERROR : 作为字典不能添加列表元素");
                    }else{
                        throw new Exception("ERROR : " + dataPath.ToString() + " 作为字典不能添加列表元素");
                    }
                }
                setToObjectRelativePath(key_,value);
            }
        }

        public override JSONNode this[int idx_]{
            get{
                if(dataPath == null){
                    throw new Exception("ERROR : 字典不能通过序号 " + (idx_).ToString() + " 取元素");
                }else{
                    throw new Exception("ERROR : " + dataPath.ToString() + " 作为字典不能通过序号 " + (idx_).ToString() + " 取元素");
                }
            }
            set{
                if(dataPath == null){
                    throw new Exception("ERROR : 字典不能通过序号 " + (idx_).ToString() + " 设置列表元素内容");
                }else{
                    throw new Exception("ERROR : " + dataPath.ToString() + " 作为字典不能通过序号 " + (idx_).ToString() + " 设置列表元素内容");
                }
            }
        }

        public override int Count{
            get { 
                return m_Dict.Count; 
            }
        }

        public override void AddKeyValue(string key_, JSONNode valueJsonNode_){
            if (key_ == null){
                throw new Exception("ERROR : 键不能为零");
            }
            if (valueJsonNode_ == null){
                valueJsonNode_ = JSONNull.CreateOrGet();
            }
            JSONNode _currentJsonNodeForKey = null;
            if(m_Dict.TryGetValue(key_, out _currentJsonNodeForKey)){
                _currentJsonNodeForKey.dataPath = null;
                m_Dict[key_] = valueJsonNode_;
            }else{
                m_Dict.Add(key_, valueJsonNode_);
            }
        }

        public override JSONNode Remove(string key_){
            JSONNode _currentJsonNodeForKey = null;
            if(m_Dict.TryGetValue(key_, out _currentJsonNodeForKey)){
                JSONNode _tmpJsonNode = m_Dict[key_];
                _tmpJsonNode.dataPath = null;
                m_Dict.Remove(key_);
                return _tmpJsonNode;
            }else{
                return null;
            }
        }

        public override JSONNode Remove(int idx_){
            if(dataPath == null){
                throw new Exception("ERROR : 字典不能通过序号 " + (idx_).ToString() + " 移除元素");
            }else{
                throw new Exception("ERROR : " + dataPath.ToString() + " 作为字典不能通过序号 " + (idx_).ToString() + " 移除元素");
            }
        }

        public override JSONNode Remove(JSONNode jsonNode_){
            try{
                var _kvPair = m_Dict.Where(k => k.Value == jsonNode_).First();
                jsonNode_.dataPath = null;
                m_Dict.Remove(_kvPair.Key);
                return jsonNode_;
            }catch{
                return null;
            }
        }

        public override void Clear(){
            if(dataPath != null){
                if (m_Dict.Keys.Count > 0){
                    var _m_DictEnume = m_Dict.GetEnumerator();
                    while (_m_DictEnume.MoveNext()) {
                        JSONNode _jsNode = _m_DictEnume.Current.Value;
                        _jsNode.dataPath = null;
                    }
                    _m_DictEnume.Dispose();
                }
            }
            m_Dict.Clear();
        }

        public override JSONNode Clone(){
            var _jsonObject = new JSONObject();
            foreach (var _kvPair in m_Dict){
                _jsonObject.AddKeyValue(_kvPair.Key, _kvPair.Value.Clone());
            }
            return _jsonObject;
        }

        public override bool HasKey(string key_){
            return m_Dict.ContainsKey(key_);
        }

        public override JSONNode GetValueOrDefault(string key_, JSONNode defaultJsonNode_){
            JSONNode _jsNode;
            if (m_Dict.TryGetValue(key_, out _jsNode)){
                return _jsNode;
            }
            return defaultJsonNode_;
        }

        public override IEnumerable<JSONNode> Children{
            get{
                foreach (KeyValuePair<string, JSONNode> _kvPair in m_Dict){
                    yield return _kvPair.Value;
                }
            }
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append('{');
            bool _firstBool = true;
            if (inline){
                mode_ = JSONTextMode.Compact;
            }
            foreach (var _kvPair in m_Dict){
                if (!_firstBool){
                    sb_.Append(',');
                }
                _firstBool = false;
                if (mode_ == JSONTextMode.Indent){
                    sb_.AppendLine();
                }
                if (mode_ == JSONTextMode.Indent){
                    sb_.Append(' ', indent_ + indentInc_);
                }
                sb_.Append('\"').Append(Escape(_kvPair.Key)).Append('\"');
                if (mode_ == JSONTextMode.Compact){
                    sb_.Append(':');
                }else{
                    sb_.Append(" : ");
                }
                _kvPair.Value.WriteToStringBuilder(sb_, indent_ + indentInc_, indentInc_, mode_);
            }
            if (mode_ == JSONTextMode.Indent){
                sb_.AppendLine().Append(' ', indent_);
            }
            sb_.Append('}');
        }

    }
    // End of JSONObject

    public partial class JSONString : JSONNode{
        private string m_Data;

        public override JSONNodeType Tag {
            get { 
                return JSONNodeType.String; 
            } 
        }
        public override bool IsString { 
            get { 
                return true; 
            } 
        }

        public override Enumerator GetEnumerator() { 
            return new Enumerator(); 
        }


        public override string Value{
            get { 
                return m_Data; 
            }
            set{
                m_Data = value;
            }
        }

        public JSONString(string str_){
            m_Data = str_;
        }
        public override JSONNode Clone(){
            return new JSONString(m_Data);
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append('\"').Append(Escape(m_Data)).Append('\"');
        }
        public override bool Equals(object obj_){
            if (base.Equals(obj_)){
                return true;
            }
            string _str = obj_ as string;
            if (_str != null){
                return m_Data == _str;
            }
                
            JSONString _jsonString = obj_ as JSONString;
            if (_jsonString != null){
                return m_Data == _jsonString.m_Data;
            }
                
            return false;
        }
        public override int GetHashCode(){
            return m_Data.GetHashCode();
        }
        public override void Clear(){
            m_Data = "";
        }
    }
    // End of JSONString

    public partial class JSONNumber : JSONNode{
        private double m_Data;

        public override JSONNodeType Tag { 
            get { 
                return JSONNodeType.Number; 
            } 
        }
        public override bool IsNumber { 
            get { 
                return true; 
            } 
        }
        public override Enumerator GetEnumerator() { 
            return new Enumerator(); 
        }

        public override string Value{
            get { 
                return m_Data.ToString(CultureInfo.InvariantCulture);
            }
            set{
                double _v;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _v)){
                    m_Data = _v;
                }
                    
            }
        }

        public override double AsDouble{
            get {
                return m_Data; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                m_Data = value; 
            }
        }
        public override long AsLong{
            get {
                return (long)m_Data; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                m_Data = value; 
            }
        }
        public override ulong AsULong{
            get {
                return (ulong)m_Data; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                m_Data = value; 
            }
        }

        public JSONNumber(double aData){
            m_Data = aData;
        }

        public JSONNumber(string aData){
            Value = aData;
        }

        public override JSONNode Clone(){
            return new JSONNumber(m_Data);
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append(Value);
        }
        // SAMPLE - C# is numeric object.
        public static bool IsNumeric(object value_){
            return value_ is int || value_ is uint || value_ is float || value_ is double || value_ is decimal || value_ is long || value_ is ulong || value_ is short || value_ is ushort || value_ is sbyte || value_ is byte;
        }
        public override bool Equals(object obj_){
            if (obj_ == null){
                return false;
            }   
            if (base.Equals(obj_)){
                return true;
            }   
            JSONNumber _num = obj_ as JSONNumber;
            if (_num != null){
                return m_Data == _num.m_Data;
            }
            if (IsNumeric(obj_)){
                return Convert.ToDouble(obj_) == m_Data;
            }
            return false;
        }
        public override int GetHashCode(){
            return m_Data.GetHashCode();
        }
        public override void Clear(){
            m_Data = 0;
        }
    }
    // End of JSONNumber

    public partial class JSONBool : JSONNode{
        protected bool m_Data;

        public override JSONNodeType Tag {
            get {
                return JSONNodeType.Boolean;
            } 
        }
        public override bool IsBoolean { 
            get {
                return true; 
            } 
        }
        public override Enumerator GetEnumerator() { 
            return new Enumerator(); 
        }

        public override string Value{
            get {
                return m_Data.ToString(); 
            }
            set{
                bool _v;
                if (bool.TryParse(value, out _v)){
                    m_Data = _v;
                }   
            }
        }
        public override bool AsBool{
            get { 
                return m_Data; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                m_Data = value; 
            }
        }

        public JSONBool(bool aData){
            m_Data = aData;
        }

        public JSONBool(string aData){
            Value = aData;
        }

        public override JSONNode Clone(){
            return new JSONBool(m_Data);
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append((m_Data) ? "true" : "false");
        }
        public override bool Equals(object obj_){
            if (obj_ == null){
                return false;
            }
            if (obj_ is bool){
                return m_Data == (bool)obj_;
            }   
            return false;
        }
        public override int GetHashCode(){
            return m_Data.GetHashCode();
        }
        public override void Clear(){
            m_Data = false;
        }
    }
    // End of JSONBool

    public partial class JSONNull : JSONNode{
        static JSONNull m_StaticInstance = new JSONNull();
        public static bool reuseSameInstance = false;
        public static JSONNull CreateOrGet(){
            if (reuseSameInstance){
                return m_StaticInstance;
            }
            return new JSONNull();
        }
        private JSONNull() { }

        public override JSONNodeType Tag {
            get { 
                return JSONNodeType.NullValue; 
            } 
        }
        public override bool IsNull { 
            get { 
                return true; 
            } 
        }
        public override Enumerator GetEnumerator() { 
            return new Enumerator(); 
        }

        public override string Value{
            get { 
                return "null"; 
            }
            set { 

            }
        }
        public override bool AsBool{
            get { 
                return false; 
            }
            set { 

            }
        }

        public override JSONNode Clone(){
            return CreateOrGet();
        }

        public override bool Equals(object obj_){
            if(obj_ == null){
                return true;
            }
            if (object.ReferenceEquals(this, obj_)){
                return true;
            }
            return (obj_ is JSONNull);
        }
        public override int GetHashCode(){
            return 0;
        }

        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append("null");
        }
    }
    // End of JSONNull

    internal partial class JSONLazyCreator : JSONNode{
        private JSONNode m_Node = null;
        private string m_Key = null;
        public override JSONNodeType Tag { 
            get { 
                return JSONNodeType.None; 
            } 
        }
        public override Enumerator GetEnumerator() { 
            return new Enumerator(); 
        }

        public JSONLazyCreator(JSONNode jsonNode_){
            m_Node = jsonNode_;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode jsonNode_, string key_){
            m_Node = jsonNode_;
            m_Key = key_;
        }

        private T Set<T>(T valueOrItem_) where T : JSONNode{
            if (m_Key == null){
                m_Node.AddItem(valueOrItem_);
            }else{
                m_Node.AddKeyValue(m_Key, valueOrItem_);
            }
            m_Node = null; // Be GC friendly.
            return valueOrItem_;
        }

        public override JSONNode this[int idx_]{
            get {
                return new JSONLazyCreator(this); 
            }
            set { 
                Set(new JSONArray()).AddItem(value); 
            }
        }

        public override JSONNode this[string key_]{
            get {
                return new JSONLazyCreator(this, key_); 
            }
            set { 
                Set(new JSONObject()).AddKeyValue(key_, value); 
            }
        }

        public override void AddItem(JSONNode item_){
            Set(new JSONArray()).AddItem(item_);
        }

        public override void AddKeyValue(string key_, JSONNode item_){
            Set(new JSONObject()).AddKeyValue(key_, item_);
        }

        public static bool operator ==(JSONLazyCreator jsonLazyCreator_, object value_){
            if (value_ == null){
                return true;
            }
            return System.Object.ReferenceEquals(jsonLazyCreator_, value_);
        }

        public static bool operator !=(JSONLazyCreator jsonLazyCreator_, object value_){
            return !(jsonLazyCreator_ == value_);
        }

        public override bool Equals(object obj_){
            if (obj_ == null){
                return true;
            }   
            return System.Object.ReferenceEquals(this, obj_);
        }

        public override int GetHashCode(){
            return 0;
        }

        public override int AsInt{
            get { 
                Set(new JSONNumber(0)); 
                return 0; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Set(new JSONNumber(value)); 
            }
        }

        public override float AsFloat{
            get { 
                Set(new JSONNumber(0.0f)); 
                return 0.0f; 
            }
            set { 
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Set(new JSONNumber(value)); 
            }
        }

        public override double AsDouble{
            get { 
                Set(new JSONNumber(0.0)); 
                return 0.0; 
            }
            set { 
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Set(new JSONNumber(value)); 
            }
        }

        public override long AsLong{
            get{
                if (longAsString){
                    Set(new JSONString("0"));
                }else{
                    Set(new JSONNumber(0.0));
                }   
                return 0L;
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                if (longAsString){
                    Set(new JSONString(value.ToString()));
                }else{
                    Set(new JSONNumber(value));
                }
            }
        }

        public override ulong AsULong{
            get{
                if (longAsString){
                    Set(new JSONString("0"));
                }else{
                    Set(new JSONNumber(0.0));
                }
                return 0L;
            }
            set{
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                if (longAsString){
                    Set(new JSONString(value.ToString()));
                }else{
                    Set(new JSONNumber(value));
                }
            }
        }

        public override bool AsBool{
            get { 
                Set(new JSONBool(false)); 
                return false; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Set(new JSONBool(value)); 
            }
        }

        public override JSONArray AsArray{
            get {
                return Set(new JSONArray()); 
            }
        }

        public override JSONObject AsObject{
            get {
                return Set(new JSONObject()); 
            }
        }
        internal override void WriteToStringBuilder(StringBuilder sb_, int indent_, int indentInc_, JSONTextMode mode_){
            sb_.Append("null");
        }
    }
    // End of JSONLazyCreator
    public static class JSON{
        public static JSONNode Parse(string aJSON){
            return JSONNode.Parse(aJSON);
        }
    }
}

