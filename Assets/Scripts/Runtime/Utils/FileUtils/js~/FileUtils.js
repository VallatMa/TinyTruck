mergeInto(LibraryManager.library, {
	
    SaveInput: function (input) { 
	
        input = UTF8ToString(input);
		try { 
			cookieID = Cookies.get('idTiny');
		} catch (error) {
			console.log(error);
			cookieID = "noCookieSetForThisRun";
		}
		
		try { 
			jQuery.ajax({
				type: "POST",
				url: 'FileUtils.php',
				dataType: 'json',
				data: {line: input, cookie: cookieID},

				success: function (obj, textstatus) {
							  if( !('error' in obj) ) {
								  yourVariable = obj.result;
							  }
							  else {
								  console.log(obj.error);
							  }
						}
			});
		} catch (error) {
			console.log(error);
		}
      }
});