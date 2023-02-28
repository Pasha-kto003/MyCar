let wrapper = $(".tabs");
let tabToggle = wrapper.find(".tab-toggle");
function openTab() {
    let content = $(this).parent().next(".Tabscontent"),
        activeItems = wrapper.find(".active");
    if ($(window).width() > 991) {
        if (!$(this).hasClass('active')) {
            $(this).add(content).add(activeItems).toggleClass('active');
            wrapper.css('min-height', content.outerHeight());
        }
    } else {
        $(this).add(content).toggleClass('active');
    }
};
tabToggle.on('click', openTab);
tabToggle.first().trigger('click');