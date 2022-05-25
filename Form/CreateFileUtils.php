<?php
	// get the cookies
	$id = $_COOKIE['idTiny']; 
	$email = $_COOKIE['email']; 
	$place = $_COOKIE['place']; 
	$hand = $_COOKIE['hand']; 
	$medic = $_COOKIE['medic']; 
	$medicType = $_COOKIE['medicType']; 
	$medicTime = $_COOKIE['medicTime'];
	$date = date("Y-m-d"); 
	$timeDate = date("Y-m-d H:i:s"); 
	
	$data['id'] = $id;
	$data['email'] = $email;
	$data['date'] = $timeDate;
	$data['place'] = $place;
	$data['hand'] = $hand;
	$data['medic'] = $medic;
	$data['medicType'] = $medicType;
	$data['medicTime'] = $medicTime;
	$data['sessions'] = array();
	
	$dataJSON = json_encode($data);
	
	$fname =  $date . '-' . $id . ".json"; // generates the file name
	$file = fopen("Traces/" . $fname, 'w'); // creates new file
	
	fwrite($file, $dataJSON); // save it
	
	fclose($file); // close it
?>