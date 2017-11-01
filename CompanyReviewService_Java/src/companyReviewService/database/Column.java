package companyReviewService.database;

import java.util.ArrayList;

/**
 * This class represents a single column in a MySQL database table
 * @author joshua
 */
public class Column {
	
	public Column(String name, String type, ArrayList<String> mods, boolean pKey){
		this.name = name;
		this.type = type;
		this.mods = mods;
		this.pKey = pKey;
	}
	
	/**
	 * Generates a string that can be used in a MySQL CREATE query to create this column.
	 * @return string used in CREATE statements
	 */
	public String getCreateStructure(){
		String structure = name + " " + type;
		if(mods != null){
			for(int i = 0; i != mods.size(); i++){
				structure += " " + mods.get(i);
			}
		}
		return structure;
	}
	
	/**
	 * The name of the column
	 */
	private String name;
	public String getName(){ return name; }
	
	/**
	 * The datatype of the column
	 */
	private String type;
	public String getType(){ return type; }
	
	/**
	 * Any modifications to the columns, such as NOT NULL or UNIQUE
	 */
	private ArrayList<String> mods;
	public ArrayList<String> getMods(){ return mods; }
	
	/**
	 * Indicates whether or not this column is a primary key
	 */
	private boolean pKey;
	public boolean isPKey(){ return pKey; }
}
