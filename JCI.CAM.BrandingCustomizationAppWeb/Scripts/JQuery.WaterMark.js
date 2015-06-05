//Check whether InjectBrandingNavigation.js called jQueryWaterMark method or not. If not call the method.

var JCIJQueryWaterMark = {};

JCIJQueryWaterMark.ready = function (){
        jQuery.fn.extend({
            inputWatermark: function () {
                return this.each(function () {
                    // retrieve the value of the ‘placeholder’ attribute
                    var watermarkText = $.trim($(this).attr('value'));
                    var $this = $(this);
                    if ($this.val() === '') {
                        $this.val(watermarkText);
                        // give the watermark a translucent look
                        $this.css({ 'opacity': '0.65' });
                    }



                    $this.blur(function () {
                        if ($.trim($this.val()) === '') {
                            // If the text is empty put the watermark
                            // back

                            $this.val(watermarkText);
                            // give the watermark a translucent look
                            $this.css({ 'opacity': '0.65' });
                        }
                    });

                    $this.focus(function () {
                        setTimeout(
                            function () {
                                if ($.trim($this.val()) === watermarkText) {
                                    $this.val('');
                                    $this.css({ 'opacity': '1.0' });
                                }
                            }
                        , 1);
                    });
                });
            }
        });
    };


