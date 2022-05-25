<?php

if(!empty($_POST['line'])){
	$data = $_POST['line'];
	$id = $_POST['cookie'];
	$json_data = json_encode($data);
	$fname = $id . ".json"; //generates name
	$file = fopen("Traces/" . $fname, 'a'); //creates new file or open existing
	fwrite($file, $json_data);
	fclose($file);
}

?>