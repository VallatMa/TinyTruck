
//jQuery time
var current_fs, next_fs, previous_fs; //fieldsets
var left, opacity, scale; //fieldset properties which we will animate
var animating; //flag to prevent quick multi-click glitches

$(".next").click(function(){
    current_fs = $(this).parent();
    next_fs = $(this).parent().next();
	Next();
});

$(".nextFormHash").click(function(){
	if( $('#formHash').smkValidate() ){
		current_fs = $(this).parent().parent();
		next_fs = $(this).parent().parent().next();
		Next();
	}
});

$(".nextFormFragen").click(function(){
	if( $('#formFragen').smkValidate() ){
		current_fs = $(this).parent().parent();
		next_fs = $(this).parent().parent().next();
		Next();
	}
});

$(".previous").click(function(){
    current_fs = $(this).parent();
    previous_fs = $(this).parent().prev();
	Previous();
});

$(".previousForm").click(function(){
    current_fs = $(this).parent().parent();
    previous_fs = $(this).parent().parent().prev();
	Previous();
});

function Next() {
    if(animating) return false;
    animating = true;
  
    /*current_fs = $(this).parent();
    next_fs = $(this).parent().next();*/
    
    //activate next step on progressbar using the index of next_fs
    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
    
    //show the next fieldset
    next_fs.show(); 
    //hide the current fieldset with style
    current_fs.animate({opacity: 0}, {
        step: function(now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)
            left = (now * 2)+"%";
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({
        'transform': 'scale('+scale+')',
        'position': 'absolute'
      });
            next_fs.css({'left': left, 'opacity': opacity});
        }, 
        duration: 800, 
        complete: function(){
            current_fs.hide();
            animating = false;
        }, 
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
}

function Previous() {
    if(animating) return false;
    animating = true;
        
    //de-activate current step on progressbar
    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");
    
    //show the previous fieldset
    previous_fs.show(); 
    //hide the current fieldset with style
    current_fs.animate({opacity: 0}, {
        step: function(now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale previous_fs from 80% to 100%
            scale = 0.8 + (1 - now) * 0.2;
            //2. take current_fs to the right(50%) - from 0%
            left = ((1-now) * 2)+"%";
            //3. increase opacity of previous_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({'left': left});
            previous_fs.css({'transform': 'scale('+scale+')', 'opacity': opacity});
        }, 
        duration: 800, 
        complete: function(){
            current_fs.hide();
            animating = false;
        }, 
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
}

$(".submit").click(function(){
	
	var name = document.getElementById("name").value.toLowerCase();
	var surname = document.getElementById("surname").value.toLowerCase();
	var email = document.getElementById("email").value;
	var dateOfBirth = new Date($('#dob').val());
	var date = new Date();
	var minutes = date.getMinutes();
	var hours = date.getHours();
	var day = date.getDate();
	var month = date.getMonth() + 1;
	var year = date.getFullYear();
	
	var datetime = year + "-" + month + "-" + day + "-" + hours + "-" + minutes;
	var hash = sha256.update(name + "-" + surname + "-" + dateOfBirth.getFullYear());

	var fname =  datetime + '_' + hash + ".json"; // Get the file by name the file name
	
	var place = getSelectedCheckboxValues("check_place");
	var hand = getSelectedCheckboxValues("check_hand");
	var medic = getSelectedCheckboxValues("check_medic");
	var medicType = document.getElementById("input_medic").value;
	var medicTime = document.getElementById("time_medic").value;
	
	Cookies.set('idTiny', hash, { expires: 7 });
	Cookies.set('email', email, { expires: 7 });
	Cookies.set('place', place, { expires: 7 });
	Cookies.set('hand', hand, { expires: 7 });
	Cookies.set('medic', medic, { expires: 7 });
	Cookies.set('medicType', medicType, { expires: 7 });
	Cookies.set('medicTime', medicTime, { expires: 7 });
	Cookies.set('fileName', fname, { expires: 7 });
	
	
	jQuery.ajax({
		type: "POST",
		url: 'CreateFileUtils.php',
		dataType: 'json'
	});
	
	window.location = 'https://tiny.humantech.institute/Tiny2D.html';
	// CHANGE THE LOCATION DEPENDING OF THE LOCAL SERVER OR THE PRODUCTION SERVER
	//window.location.pathname = 'TinyFood/Tiny2D.html'; 

	return false;
});

function getSelectedCheckboxValues(name) {
    const checkboxes = document.querySelectorAll(`input[name="${name}"]:checked`);
    let values = [];
    checkboxes.forEach((checkbox) => {
        values.push(checkbox.value);
    });
    return values;
}

// the selector will match all input controls of type :checkbox
// and attach a click event handler 
$("input:checkbox").on('click', function() {
  // in the handler, 'this' refers to the box clicked on
  var $box = $(this);
  if ($box.is(":checked")) {
    // the name of the box is retrieved using the .attr() method
    // as it is assumed and expected to be immutable
    var group = "input:checkbox[name='" + $box.attr("name") + "']";
    // the checked state of the group/box on the other hand will change
    // and the current value is retrieved using .prop() method
    $(group).prop("checked", false);
    $box.prop("checked", true);
  } else {
    $box.prop("checked", false);
  }
});