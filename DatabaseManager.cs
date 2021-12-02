using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;

using System;

using static Elements;

public class DatabaseManager {

	private string connectionString = "";
	private SQLiteConnection con;
	private SQLiteCommand cmd;

	public List<Enemy> getEnemyList(string dbName, int MaximumEnemies) {
		if (!connectDB(dbName)) 
			return null;

		List<Enemy> to_return = new List<Enemy>();

		SQLiteDataReader reader = executeSQLiteRequest(dbName, "SELECT * FROM Enemies LIMIT " + MaximumEnemies.ToString());

		while (reader.Read()) {
			to_return.Add(
				new Enemy(
				reader.GetInt32(1), // damage
				reader.GetInt32(2),	// base strength
				reader.GetInt32(3), // base speed
				reader.GetInt32(4), // base evasion
				reader.GetInt32(5), // base resistance
				reader.GetInt32(6), // max hp
				reader.GetString(7), // name
				reader.GetString(8), // description
				reader.GetString(9), // immunes
				reader.GetInt32(10), // cashmoneygiven
				reader.GetInt32(11) // xpgiven
				)
			);
		}

		reader.Close();

		return to_return;

	}

	public void closeConnection() {
		con.Close();
	}

	public List<Item> getItemsList(string dbName, int maximumItems) {
		if (!connectDB(dbName)) 
			return null;

		List<Item> to_return = new List<Item>();

		SQLiteDataReader reader = executeSQLiteRequest(dbName, "SELECT * FROM Items LIMIT " + maximumItems.ToString());

		while (reader.Read()) {
			to_return.Add(
				new Item(
					reader.GetString(1),
					reader.GetString(2),
					getEffectStr(reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5)),
					reader.GetInt32(6),
					reader.GetInt32(7)
				)
			);
		}

		reader.Close();

		return to_return;

	}

	private bool connectDB(string dbname) {
		con = new SQLiteConnection(dbname);
		con.Open();
		if (con != null) {
			connectionString = dbname;
			return true;
		}
		else {
			return false;
		}
	}

	private void executeSQLiteCommandNoReturn(string dbName, string command) {
		if (!connectDB(dbName)) {
			return;
		}

		if (cmd == null) {
			cmd = new SQLiteCommand(con);
		}

		cmd.CommandText = command;
		cmd.ExecuteNonQuery();
	}

	private SQLiteDataReader executeSQLiteRequest(string dbName, string request) {
		if (!connectDB(dbName)) {
			return null;
		}

		if (cmd == null) {
			cmd = new SQLiteCommand(con);
		}

		cmd.CommandText = request;
		return cmd.ExecuteReader();
	}

}