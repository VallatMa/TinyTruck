<?php

if(!empty($_POST['line'])){
	$data = $_POST['line']; // get the JSON
	$id = $_POST['cookie']; // get the cookie
	$fname = $id . ".json"; // generates the file name
	$file = fopen("Traces/" . $fname, 'a'); // creates new file
	fwrite($file, $data); // save it
	fclose($file); // close it
}

?>