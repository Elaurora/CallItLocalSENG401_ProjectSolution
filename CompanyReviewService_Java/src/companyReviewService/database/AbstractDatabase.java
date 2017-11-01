package companyReviewService.database;

import java.sql.*;
import java.util.ArrayList;

/**
 * This class respresents an arbirary SQL database, and has functions to create the database and to connect to it
 * @author joshua walters
 *
 */
public abstract class AbstractDatabase {
	protected AbstractDatabase() throws ClassNotFoundException, SQLException{
		createDB();
	}
	
	/**
	 * Create's the database and its tables using MySql queries. 
	 * If the database already exists this function will not do anything
	 * @throws ClassNotFoundException - If the Driver library is missing
	 * @throws SQLException - If there is an error creating the database.
	 */
	private void createDB() throws ClassNotFoundException, SQLException{
		Connection connection = null;
		Statement statement = null;
		String query = null;
		
		try{
			connection = getConnection();
			query = "CREATE DATABASE " + getDBName() + ";";
			statement = connection.createStatement();
			statement.execute(query);
			statement.close();
			
			//If the database already existed an exception would have been caught.
			ArrayList<Table> tables = getTables();
			
			//Create the database tables
			for(int i = 0; i != tables.size(); i++){
				Table current = tables.get(i);
				query = current.getCreateStructure();
				statement = connection.createStatement();
				statement.execute(query);
				statement.close();
			}
			
			//NEXT THINGS TO DO: 
			// 1. Continue to test the create function until confident it is working, 
			// 2. Figure out how to properly serialize getReviews responses
			// 3. Complete the remainder of this service.
			
			connection.close();
		}
		catch(SQLException e){
			if(e.getErrorCode() != 1007){//1007 means database already exists
				throw e;
			}
			return;
		}
		finally{
			try{
				if(connection != null && !connection.isClosed()){
					connection.close();
				}
				if(statement != null && !statement.isClosed()){
					statement.close();
				}
			}
			catch(SQLException e){ }
		}
	}
	
	/**
	 * Opens a connection to the root mysql database and returns it
	 * @return Connection object
	 * @throws SQLException - If there is an error connecting to the database
	 * @throws ClassNotFoundException - If the Driver library is missing
	 */
	protected Connection getConnection() throws SQLException, ClassNotFoundException{
		Class.forName("com.mysql.jdbc.Driver");
		return DriverManager.getConnection("jdbc:mysql://localhost:3306/mysql", UID, password);
	}
	
	protected abstract String getDBName();
	protected abstract ArrayList<Table> getTables();
	
	
	private final String UID = "root";
	private final String password = "abc123";
}