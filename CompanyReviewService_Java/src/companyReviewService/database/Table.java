package companyReviewService.database;

import java.util.ArrayList;

/**
 * This class respresents the schema for a table in a MySQL database.
 * @author joshua walters
 */
public class Table {
	public Table(String databaseName, String tableName, ArrayList<Column> columns){
		this.databaseName = databaseName;
		this.tableName = tableName;
		this.columns = columns;
	}
	
	/**
	 * Generates a string that can be used as a MySQL statement to create this table in a MySQL Database
	 * @return String that can be used as a create statement
	 */
	public String getCreateStructure(){
		String structure = "CREATE TABLE " + databaseName + "." + tableName + "(";
		ArrayList<Integer> pKeys = new ArrayList<Integer>();
		int i = 0;
		
		while(true){
			Column current = columns.get(i++);
			structure += current.getCreateStructure();
			if(current.isPKey()){
				pKeys.add(i - 1);
			}
			if(i == columns.size()){
				break;
			}
			structure += ",";
		}
		if(pKeys.size() != 0){
			i = 0;
			structure += ",PRIMARY KEY(";
			while(true){
				if(i == (pKeys.size() - 1)){
					break;
				}
				structure += columns.get(pKeys.get(i++)).getName() + ", ";
			}
			structure += columns.get(pKeys.get(i)).getName() + ")";
		}
		
		structure += ");";
		
		return structure;
	}
	
	/**
	 * The name of the database this table belongs to
	 */
	private String databaseName;
	public String getDatabaseName(){ return databaseName; }
	
	/**
	 * The name of the table
	 */
	private String tableName;
	public String getTableName(){ return tableName; }
	
	/**
	 * A list of the columns that make up this table
	 */
	private ArrayList<Column> columns;
	public ArrayList<Column> getColumns(){ return columns; }
}
