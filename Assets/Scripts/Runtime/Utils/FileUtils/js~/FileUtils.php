<?php

if(!empty($_POST['line'])){
	$data = $_POST['line'];
	$id = $_POST['cookie'];
	$json_data = json_encode($data);
	$fname = $id . ".json";//generates random name
	$file = fopen("Traces/" . $fname, 'a');//creates new file
	fwrite($file, $json_data);
	fclose($file);
}

?>