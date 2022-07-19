<?php

if(!empty($_POST['line'])){
	
	$data = $_POST['line']; // get the JSON
	
	// get the cookies
	$fname = $_COOKIE['fileName'];
	
	//$dataFile = json_decode(file_get_contents($fname), TRUE); // Get the content and decode to JSON
	$dataIn = json_decode(stripslashes($data)); // Get the content and decode to JSON
	
	$file_handle  = fopen("Traces/" . $fname, 'r'); // creates new file
	
	while(!feof($file_handle)){
		$line_of_text = fgets($file_handle);
		$dataFile = json_decode($line_of_text, true);
	}
	
	array_push($dataFile['sessions'], $dataIn); // Push the session to the array sessions
	
	//file_put_contents($file, json_encode($dataFile)); // Put the content and encode to JSON
	
	$file_handle  = fopen("Traces/" . $fname, 'w'); // creates new file
	
	fwrite($file_handle , json_encode($dataFile)); // save it
	
	fclose($file_handle ); // close it
}

?>