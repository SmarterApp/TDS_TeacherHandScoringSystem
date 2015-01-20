#region License
// /*******************************************************************************                                                                                                                                    
//  * Educational Online Test Delivery System                                                                                                                                                                       
//  * Copyright (c) 2014 American Institutes for Research                                                                                                                                                              
//  *                                                                                                                                                                                                                  
//  * Distributed under the AIR Open Source License, Version 1.0                                                                                                                                                       
//  * See accompanying file AIR-License-1_0.txt or at                                                                                                                                                                  
//  * http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf                                                                                                                                                 
//  ******************************************************************************/ 
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace TSS.Data.Sql
{
    /// <summary>
    /// This is a DataReader that sets any null values and missing columns to default value
    /// </summary>
    /// <remarks>
    /// Performance tips when using SqlDataReader along with this wrapper:
    /// 
    /// Use sequential access of the SqlDataReader. By default, the SqlDataReader loads an 
    /// entire row into memory with each Read. This allows for random access of columns within 
    /// the current row. If this random access is not necessary, for increased performance, 
    /// pass CommandBehavior.SequentialAccess to the call to ExecuteReader. This changes the 
    /// default behavior of the SqlDataReader to only load data into memory when it is requested. 
    /// Note that once you have read past a returned column, you can no longer read its value.
    /// 
    /// Call Cancel on a SqlDataReader operation that still has pending rows to be fetched. 
    /// Otherwise, calling close on the connection will first finish off any pending tasks and 
    /// then close the connection.
    /// </remarks>
    public class ColumnReader : IColumnReader
    {
        private readonly IDataReader _reader;
        private Dictionary<string, int> _fields = null;
        private bool _fixNulls = false;
        private bool _fixMissing = false;

        public bool HasColumn(string name)
        {
            return GetOrdinal(name) != -1;
        }

        public bool HasValue(string name)
        {
            return this.HasColumn(name) && !this.IsDBNull(name);
        }

        /// <summary>
        /// Current readers columns
        /// </summary>
        public List<string> Columns
        {
            get
            {
                List<string> columns = new List<string>();

                int fieldCount = _reader.FieldCount;

                for (int i = 0; i < fieldCount; i++)
                {
                    columns.Add(_reader.GetName(i));
                }

                return columns;
            }
        }


        /// <summary>
        /// Check if fields are null and provide a default value
        /// </summary>
        public bool FixNulls
        {
            get { return _fixNulls; }
            set { _fixNulls = value; }
        }

        /// <summary>
        /// Perform a check if field exists and provide a default value
        /// </summary>
        public bool FixMissing
        {
            get { return _fixMissing; }
            set { _fixMissing = value; }
        }

        /// <summary>
        /// Initializes the ColumnReader object to use data from
        /// the provided DataReader object.
        /// </summary>
        /// <param name="reader">The source DataReader object containing the data.</param>
        public ColumnReader(IDataReader reader)
        {
            this._reader = reader;
        }

        /// <summary>
        /// Initializes the ColumnReader object to use data from
        /// the provided DataReader object.
        /// </summary>
        /// <param name="reader">The source DataReader object containing the data.</param>
        /// <param name="fixNulls">Check if fields are null and provide a default value</param>
        /// <param name="fixMissing">Perform a check if field exists and provide a default value</param>
        public ColumnReader(IDataReader reader, bool fixMissing, bool fixNulls)
        {
            this._reader = reader;
            this._fixNulls = fixNulls;
            this._fixMissing = fixMissing;
        }

        /// <summary>
        /// Get a reference to the underlying data reader
        /// object that actually contains the data from
        /// the data source.
        /// </summary>
        protected IDataReader DataReader
        {
            get { return _reader; }
        }

        /// <summary>
        /// Gets a string value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns empty string for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public string GetString(string name)
        {
            return GetString(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a string value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns empty string for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetString(int i)
        {
            // return GetGenericValue<string>(i) ?? String.Empty;
            return GetGenericValue<string>(i);
        }

        /// <summary>
        /// Gets a value of type <see cref="System.Object" /> from the datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public object GetValue(string name)
        {
            return GetValue(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a value of type <see cref="System.Object" /> from the datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual object GetValue(int i)
        {
            return GetGenericValue<object>(i);
        }

        /// <summary>
        /// Gets an integer from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public int GetInt32(string name)
        {
            return GetInt32(GetOrdinal(name));
        }

        /// <summary>
        /// Gets an integer from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual int GetInt32(int i)
        {
            return GetGenericValue<int>(i);
        }

        /// <summary>
        /// Gets a double from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public double GetDouble(string name)
        {
            return GetDouble(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a double from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual double GetDouble(int i)
        {
            return GetGenericValue<double>(i);
        }

        /// <summary>
        /// Gets a Guid value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Guid.Empty for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public Guid GetGuid(string name)
        {
            return GetGuid(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Guid value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Guid.Empty for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Guid GetGuid(int i)
        {
            return GetGenericValue<Guid>(i);
        }

        /// <summary>
        /// Gets a boolean value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="false" /> for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public bool GetBoolean(string name)
        {
            return GetBoolean(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a boolean value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="false" /> for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual bool GetBoolean(int i)
        {
            return GetGenericValue<bool>(i);
        }

        /// <summary>
        /// Gets a date value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public virtual DateTime GetDateTime(string name)
        {
            return GetDateTime(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a date value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns DateTime.MinValue for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual DateTime GetDateTime(int i)
        {
            return GetGenericValue<DateTime>(i);
        }

        /// <summary>
        /// Gets a decimal value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public decimal GetDecimal(string name)
        {
            return GetDecimal(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a decimal value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual decimal GetDecimal(int i)
        {
            return GetGenericValue<decimal>(i);
        }


        /// <summary>
        /// Gets a Single value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public float GetFloat(string name)
        {
            return GetFloat(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Single value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual float GetFloat(int i)
        {
            return GetGenericValue<float>(i);
        }

        /// <summary>
        /// Gets a Short value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public short GetInt16(string name)
        {
            return GetInt16(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Short value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual short GetInt16(int i)
        {
            return GetGenericValue<short>(i);
        }

        /// <summary>
        /// Gets a Long value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public Int64 GetInt64(string name)
        {
            return GetInt64(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a Long value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Int64 GetInt64(int i)
        {
            return GetGenericValue<Int64>(i);
        }


        /// <summary>
        /// Gets a char value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Char.MinValue for null. You cannot use GetChar to read a VarChar 
        /// column in chunks if CommandBehavior is set to SequentialAccess.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public char GetChar(string name)
        {
            return GetChar(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a char value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns Char.MinValue for null. You cannot use GetChar to read a VarChar 
        /// column in chunks if CommandBehavior is set to SequentialAccess.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual char GetChar(int i)
        {
            char[] myChar = new char[1];
            long charRead = _reader.GetChars(i, 0, myChar, 0, 1);

            // TODO: Look into what happens with NULL's situation here, I think it should return 0
            if (charRead == 0)
            {
                return char.MinValue;
            }
            else
            {
                return myChar[0];
            }
        }

        /// <summary>
        /// Invokes the GetChars method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public Int64 GetChars(string name, Int64 fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            return GetChars(GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Invokes the GetChars method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public virtual Int64 GetChars(int i, Int64 fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            if (_reader.IsDBNull(i))
                return 0;
            else
                return _reader.GetChars(i, fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Gets a byte value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        public byte GetByte(string name)
        {
            return GetByte(GetOrdinal(name));
        }

        /// <summary>
        /// Gets a byte value from the datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual byte GetByte(int i)
        {
            return GetGenericValue<byte>(i);
        }

        /// <summary>
        /// Invokes the GetBytes method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="name">Name of the column containing the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public Int64 GetBytes(string name, Int64 fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            return GetBytes(GetOrdinal(name), fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Invokes the GetBytes method of the underlying datareader.
        /// </summary>
        /// <remarks>
        /// Returns 0 for null.
        /// </remarks>
        /// <param name="i">Ordinal column position of the value.</param>
        /// <param name="buffer">Array containing the data.</param>
        /// <param name="bufferOffset">Offset position within the buffer.</param>
        /// <param name="fieldOffset">Offset position within the field.</param>
        /// <param name="length">Length of data to read.</param>
        public virtual Int64 GetBytes(int i, Int64 fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            if (_reader.IsDBNull(i))
                return 0;
            else
                return _reader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Invokes the GetData method of the underlying datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public IDataReader GetData(string name)
        {
            return GetData(GetOrdinal(name));
        }

        /// <summary>
        /// Invokes the GetData method of the underlying datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual IDataReader GetData(int i)
        {
            return _reader.GetData(i);
        }

        /// <summary>
        /// Invokes the GetDataTypeName method of the underlying datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public string GetDataTypeName(string name)
        {
            return GetDataTypeName(GetOrdinal(name));
        }

        /// <summary>
        /// Invokes the GetDataTypeName method of the underlying datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetDataTypeName(int i)
        {
            return _reader.GetDataTypeName(i);
        }

        /// <summary>
        /// Invokes the GetFieldType method of the underlying datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public Type GetFieldType(string name)
        {
            return GetFieldType(GetOrdinal(name));
        }

        /// <summary>
        /// Invokes the GetFieldType method of the underlying datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual Type GetFieldType(int i)
        {
            return _reader.GetFieldType(i);
        }

        /// <summary>
        /// Invokes the GetName method of the underlying datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual string GetName(int i)
        {
            return _reader.GetName(i);
        }

        /// <summary>
        /// Gets an ordinal value from the datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public int GetOrdinal(string name)
        {
            if (this._fields == null)
            {
                int fieldCount = _reader.FieldCount;

                _fields = new Dictionary<string, int>(fieldCount, StringComparer.CurrentCultureIgnoreCase);

                for (int i = 0; i < fieldCount; i++)
                {
                    _fields[_reader.GetName(i)] = i;
                }
            }
            
            int index;
            
            if (_fields.TryGetValue(name, out index))
            {
                return index;                
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Invokes the GetValues method of the underlying datareader.
        /// </summary>
        /// <param name="values">An array of System.Object to
        /// copy the values into.</param>
        public int GetValues(object[] values)
        {
            return _reader.GetValues(values);
        }

        /// <summary>
        /// Returns the IsClosed property value from the datareader.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _reader.IsClosed;
            }
        }

        /// <summary>
        /// Invokes the IsDBNull method of the underlying datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i);
        }

        /// <summary>
        /// Invokes the IsDBNull method of the underlying datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public virtual bool IsDBNull(string name)
        {
            return this.IsDBNull(GetOrdinal(name));
        }

        /// <summary>
        /// Returns a value from the datareader.
        /// </summary>
        /// <param name="name">Name of the column containing the value.</param>
        public object this[string name]
        {
            get
            {
                return _reader[GetOrdinal(name)];
            }
        }

        /// <summary>
        /// Returns a value from the datareader.
        /// </summary>
        /// <param name="i">Ordinal column position of the value.</param>
        public virtual object this[int i]
        {
            get
            {
                return _reader[i];
            }
        }

        /// <summary>
        /// Returns the RecordsAffected property value from the underlying datareader.
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return _reader.RecordsAffected;
            }
        }

        /// <summary>
        /// Invokes the GetSchemaTable method of the underlying datareader.
        /// </summary>
        public DataTable GetSchemaTable()
        {
            return _reader.GetSchemaTable();
        }

        /// <summary>
        /// Reads the next row of data from the datareader.
        /// </summary>
        public bool Read()
        {
            return _reader.Read();
        }

        /// <summary>
        /// Moves to the next result set in the datareader.
        /// </summary>
        public bool NextResult()
        {
            _fields = null; // reset the cached fields since we are moving to new record
            return _reader.NextResult();
        }

        /// <summary>
        /// Closes the datareader.
        /// </summary>
        public void Close()
        {
            _fields = null;
            _reader.Close();
        }

        /// <summary>
        /// Returns the depth property value from the datareader.
        /// </summary>
        public int Depth
        {
            get
            {
                return _reader.Depth;
            }
        }

        /// <summary>
        /// Returns the FieldCount property from the datareader.
        /// </summary>
        public int FieldCount
        {
            get
            {
                return _reader.FieldCount;
            }
        }

        /// <summary>
        /// A generic ordinal value reader
        /// </summary>
        public T GetGenericValue<T>(int i)
        {
            if (i == -1)
            {
                // if the field is missing and we are fixing this then return the default value for this value type
                if (this._fixMissing) return default(T);
                
                throw new IndexOutOfRangeException();
            }

            object value = _reader[i];

            // if the field is null and we are fixing this then return the default value for this value type
            if (_fixNulls && value == DBNull.Value) return default(T);
            
            try
            {
                return (T)value;
            }
            catch(InvalidCastException ice)
            {
                if (value == DBNull.Value)
                {
                    throw new SqlNullValueException("The column '" + _reader.GetName(i) + "' contains a NULL value: " + ice.Message);
                }
                
                throw new InvalidCastException("There was an error casting the column '" + _reader.GetName(i) + "': " + ice.Message);
            }
        }

        /// <summary>
        /// A generic name value reader
        /// </summary>
        public T GetGenericValue<T>(string name)
        {
            return GetGenericValue<T>(GetOrdinal(name));
        }

        /// <summary>
        /// A nullable ordinal reader that returns a value
        /// </summary>
        public T? GetNullableValue<T>(int i) where T : struct
        {
            if (i == -1) return null;

            object value = _reader[i];

            if (value == DBNull.Value) return null;
            
            try
            {
                return (T?)value;
            }
            catch(InvalidCastException ice)
            {
                throw new InvalidCastException("There was an error casting the column '" + _reader.GetName(i) + "': " + ice.Message);
            }
        }

        /// <summary>
        /// A nullable ordinal reader that returns a value
        /// </summary>
        public T? GetNullableValue<T>(string i) where T : struct
        {
            return GetNullableValue<T>(GetOrdinal(i));
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called by
        /// the public Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // free unmanaged resources when explicitly called
                    _reader.Dispose();
                }

                // free shared unmanaged resources
            }
            _disposedValue = true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Object finalizer.
        /// </summary>
        ~ColumnReader()
        {
            Dispose(false);
        }

        #endregion

    }
}
