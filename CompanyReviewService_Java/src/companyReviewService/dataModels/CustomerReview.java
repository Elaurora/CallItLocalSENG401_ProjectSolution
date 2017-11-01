package companyReviewService.dataModels;

/**
 * This class contains all of the information needed to represent a customer's review of a company
 * @author joshua walters
 */
public class CustomerReview {
	public CustomerReview(String jsonEncoding) throws Exception{
		parseJSONString(jsonEncoding);
	}
	
	public CustomerReview(String companyName, String username, String review, int stars, int timestamp){
		this.companyName = companyName;
		this.username = username;
		this.review = review;
		this.stars = stars;
		this.timestamp = timestamp;
	}
	
	/**
	 * Parses a JSON string representation of this object such that this object contains the information contained in the string 
	 * @param encoding - The JSON string representation
	 * @throws Exception - If the given string is not a valid format
	 */
	public void parseJSONString(String encoding) throws Exception{
		int companyNameLocation = encoding.indexOf("companyName");
		int usernameLocation = encoding.indexOf("username");
		int reviewLocation = encoding.indexOf("review");
		int starsLocation = encoding.indexOf("stars");
		int timestampLocation = encoding.indexOf("timestamp");
		
		int timestampLength = 0;
		int starsLength = 0;
		
		//First check to make sure all of the json objects exist in the given string
		if(usernameLocation == -1 || reviewLocation == -1
				|| starsLocation == -1 || timestampLocation == -1
				|| companyNameLocation == -1){
			throw new Exception("Invalid jsonString given");
		}
		
		//Then adjust the indicies to point to the values we wish to read
		companyNameLocation += "companyName\":\"".length();
		usernameLocation += "username\":\"".length();
		reviewLocation += "review\":\"".length();
		starsLocation += "stars\":".length();
		timestampLocation += "timestamp\":".length();
		
		//Figure out the number of digits in the numbers corresponding to the timestamp and stars
		while(Character.isDigit(encoding.charAt(timestampLocation + timestampLength++ + 1))){}
		while(Character.isDigit(encoding.charAt(starsLocation + starsLength++ + 1))){}
		
		//Finally, read the values
		this.companyName = encoding.substring(companyNameLocation, encoding.indexOf('"', companyNameLocation));
		this.username = encoding.substring(usernameLocation, encoding.indexOf('"', usernameLocation));
		this.review = encoding.substring(reviewLocation, encoding.indexOf('"', reviewLocation));
		this.timestamp = Integer.parseInt(encoding.substring(timestampLocation, timestampLocation + timestampLength));
		this.stars = Integer.parseInt(encoding.substring(starsLocation, starsLocation + starsLength));
	}
	
	/**
	 * Converts the current state of this object into a JSON string representation
	 * @return A JSON string representation of this objects current state
	 */
	public String convertToJson(){
		String jsonEncoding = "{";
		jsonEncoding += "\"companyName\":\"" + getCompanyName() + "\"" + ",";
		jsonEncoding += "\"username\":\"" + getUsername() + "\"" + ",";
		jsonEncoding += "\"review\":\"" + getReview() + "\"" + ",";
		jsonEncoding += "\"stars\":" + String.valueOf(getStars()) + ",";
		jsonEncoding += "\"timestamp\":" + String.valueOf(getTimestamp());
		jsonEncoding += "}";
		return jsonEncoding;
	}
	
	/**
	 * The name of the company the review is about
	 */
	private String companyName = "";
	public String getCompanyName(){ return companyName; }
	
	/**
	 * The username of the user who wrote the review
	 */
	private String username = "";
	public String getUsername(){ return username; }
	
	/**
	 * The review itself
	 */
	private String review = "";
	public String getReview(){ return review; }
	
	/**
	 * The number of stars given to the company, between 0 and 5
	 */
	private int stars = -1;
	public int getStars(){ return stars; }
	
	/**
	 * A Unix timestamp indicating when the review was written
	 */
	private int timestamp = -1;
	public int getTimestamp(){ return timestamp; }
}