package companyReviewService.database;

import companyReviewService.dataModels.*;

import java.sql.*;
import java.util.ArrayList;

/**
 * This class represents the database for the Company Review Service, and contains functionality to interact with
 * the database in a safe and reliable manner
 * @author joshua walters
 */
public class CompanyReviewDatabase extends AbstractDatabase {
	
	/**
	 * This constructor is private because this class follows the singleton design pattern.
	 * @throws SQLException - If there is an error attempting to create the database
	 * @throws ClassNotFoundException - If the Driver library is missing
	 */
	private CompanyReviewDatabase() throws SQLException, ClassNotFoundException{
		super();
	}
	
	/**
	 * Gets the singleton isntance of the database. If it does not exist it will be created upon the first time this function is called
	 * @return The singleton instance of the database.
	 * @throws SQLException - If there is an error accessing the database
	 * @throws ClassNotFoundException - If the Driver library is missing
	 */
	public static CompanyReviewDatabase getInstance() throws SQLException, ClassNotFoundException{
		if(instance == null){
			instance = new CompanyReviewDatabase();
		}
		return instance;
	}
	
	/**
	 * Queries the database for a list of reviews for a specific company as outlined in the request object
	 * @param request - The request object, contains information about the request
	 * @return A response object containing a list of reviews written for a specific company
	 * @throws SQLException - If there is an error accessing the database
	 * @throws ClassNotFoundException - If the Driver library is missing
	 */
	public GetCompanyReviewsResponse getCompanyReviews(GetCompanyReviewsRequest request) throws SQLException, ClassNotFoundException{
		ArrayList<CustomerReview> reviews = new ArrayList<CustomerReview>();
		
		Connection connection = null;
		Statement statement = null;
		String query;
		
		connection = getConnection();
		statement = connection.createStatement();
		query = "SELECT r.username, r.timestamp, r.stars, r.review "
				+ "FROM " + getDBName() + "." + reviewsTable.getTableName() + " AS r "
				+ "WHERE (r.companyname='" + request.getCompanyName() + "');"; 
		ResultSet results = statement.executeQuery(query);
		
		while(results.next()){
			reviews.add(new CustomerReview(
				request.getCompanyName(),
				results.getString("username"),
				results.getString("review"),
				results.getInt("stars"),
				results.getInt("timestamp")
			));
		}
		
		results.close();
		statement.close();
		connection.close();
		
		return new GetCompanyReviewsResponse(reviews, "Success", true);
	}
	
	/**
	 * Saves a newly written Customer Review to the database
	 * @param request - The request object, contains information about the review to be saved
	 * @return A String response indicating success or failure, and an error message if relevant.
	 */
	public String saveCompanyReview(SaveCompanyReviewRequest request){
		Connection connection = null;
		Statement statement = null;
		String query;
		CustomerReview review = request.getReview();
		
		if(review == null){
			return "Error: Invalid review given, cannot save to database.";
		}
		
		try{
			connection = getConnection();
			statement = connection.createStatement();
			query = "INSERT INTO " + getDBName() + "." + reviewsTable.getTableName()
					+ "(companyName, username, timestamp, stars, review) "
					+ "VALUES('" + review.getCompanyName() + "', '" + review.getUsername() + "', '"
					+ String.valueOf(review.getTimestamp()) + "', '" + String.valueOf(review.getStars()) + "', '"
					+ review.getReview() + "');";
			statement.execute(query);
			statement.close();
			connection.close();
		}
		catch(ClassNotFoundException e){
			return "Failure: " + e.getMessage();
		}
		catch(SQLException e){
			return "Failure: " + e.getMessage();
		}
		finally{
			try {
				if(connection != null && !connection.isClosed()){
					connection.close();
				}
			} catch (SQLException e) {}
		}
		return "Success";
	}
	
	protected String getDBName(){
		return dbName;
	}
	
	@SuppressWarnings("serial")
	protected ArrayList<Table> getTables(){
		return new ArrayList<Table>(){{
			add(reviewsTable);
		}};
	}
	
	/**
	 * The singleton instance of the CompanyReviewDatabase class
	 */
	private static CompanyReviewDatabase instance = null;
	
	/**
	 * The name of the database
	 */
	private static final String dbName = "companyreviewservicedb";
	
	/**
	 * This object defines the schema of the reviews table 
	 */
	@SuppressWarnings("serial")
	private static final Table reviewsTable = new Table(dbName, "reviews",
		new ArrayList<Column>(){{
			add(new Column("companyname", "VARCHAR(50)",
				new ArrayList<String>(){{
					add("NOT NULL");
				}}, true));
			
			add(new Column("username", "VARCHAR(50)",
				new ArrayList<String>(){{
					add("NOT NULL");
				}}, true));
			
			add(new Column("timestamp", "INT(32)",
				new ArrayList<String>(){{
					add("NOT NULL");
				}}, true));
			
			add(new Column("stars", "INT(8)",
				new ArrayList<String>(){{
					add("NOT NULL");
				}}, false));
			
			add(new Column("review", "VARCHAR(300)",
				new ArrayList<String>(){{
					add("NOT NULL");
				}}, false));
		}});
}
