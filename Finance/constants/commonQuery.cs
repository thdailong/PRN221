namespace Finance.constrants
{
    public static class commonQuery
    {


        /// <summary>
        /// Wraps the specified value in quotes for SQL.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <returns>The wrapped value.</returns>
        private static string WrapValue(string value)
        {
            return "'" + value + "'";
        }

        /// <summary>
        /// Generates an SQL INSERT query for the specified table, columns, and values.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="columns">An array of column names.</param>
        /// <param name="values">An array of values.</param>
        /// <returns>The generated SQL INSERT query.</returns>
        public static string InsertQuery(string tableName, string[] columns, string[] values)
        {
            var sql = "INSERT INTO " + tableName + "(";

            // Construct the column list
            for (int i = 0; i < columns.Length; ++i)
            {
                sql += columns[i];
                if (i != columns.Length - 1)
                    sql += ',';
            }

            sql += ") VALUES (";

            // Construct the value list
            for (int i = 0; i < values.Length; ++i)
            {
                sql += WrapValue(values[i]);
                if (i != values.Length - 1)
                    sql += ',';
            }

            sql += ')';
            return sql;
        }
        /// <summary>
        /// Generates an update query for the specified table with the given columns, values, and conditions.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="columns">The columns to update.</param>
        /// <param name="values">The new values for the columns.</param>
        /// <param name="columnsCondition">The columns to use for the update condition.</param>
        /// <param name="valuesCondition">The values to use for the update condition.</param>
        /// <returns>The generated update query.</returns>
        public static string UpdateQuery(string tableName, string[] columns, string[] values, string[] columnsCondition, string[] valuesCondition)
        {
            var sql = "UPDATE " + tableName + " SET ";

            // Generate column-value pairs for update
            for (int i = 0; i < columns.Length; ++i)
            {
                sql += columns[i] + " = " + WrapValue(values[i]);
                if (i != columns.Length - 1) sql += ',';
            }

            sql += " WHERE ";

            // Generate column-value pairs for condition
            for (int i = 0; i < columnsCondition.Length; ++i)
            {
                sql += columnsCondition[i] + " = " + WrapValue(valuesCondition[i]);
                if (i != columnsCondition.Length - 1) sql += " AND ";
            }

            return sql;
        }


        /// <summary>
        /// Generates a DELETE query for a given table, columns, and values.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columns">An array of column names.</param>
        /// <param name="values">An array of corresponding values.</param>
        /// <returns>The generated DELETE query.</returns>
        public static string deleteQuery(string tableName, string[] columns, string[] values)
        {
            var sql = "DELETE FROM " + tableName + " WHERE ";

            // Generate WHERE clause
            for (int i = 0; i < columns.Length; ++i)
            {
                sql += columns[i] + " = " + WrapValue(values[i]);

                // Add 'AND' if not last column
                if (i != columns.Length - 1)
                {
                    sql += " AND ";
                }
            }

            return sql;
        }

        /// <summary>
        /// Generates a SQL query to retrieve data from a table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columns">An array of column names to select from the table.</param>
        /// <returns>The generated SQL query.</returns>
        public static string GetListQuery(string tableName, string[] columns)
        {
            // Initialize the SQL string
            var sql = "";

            // Check if columns array is empty
            if (columns.Length == 0)
            {
                // If columns array is empty, select all columns from the given table
                sql = "SELECT * FROM " + tableName;
            }
            else
            {
                // If columns array is not empty, select the specified columns
                sql = "SELECT ";

                // Loop through each column
                for (int i = 0; i < columns.Length; ++i)
                {
                    // Add a comma separator if it's not the first column
                    if (i > 0)
                        sql += ",";

                    // Add the column name to the SQL string
                    sql += columns[i];
                }
            }

            // Return the SQL string
            return sql;
        }

    }
}