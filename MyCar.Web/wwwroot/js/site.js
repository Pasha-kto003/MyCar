window.setTimeout(function () {
    $(".callout").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 4000);

function lerp({ x, y }, { x: targetX, y: targetY }) {
    const fraction = 0.1;
    x += (targetX - x) * fraction;
    y += (targetY - y) * fraction;
    return { x, y };
}
class Slider {
    constructor(el) {
        const imgClass = this.IMG_CLASS = 'sl-img-item';
        const textClass = this.TEXT_CLASS = 'sl-text-item';
        const activeImgClass = this.ACTIVE_IMG_CLASS = `${imgClass}-active`;
        const activeTextClass = this.ACTIVE_TEXT_CLASS = `${textClass}-active`;
        this.el = el;
        this.contentE0 = document.getElementById('slider');
        this.contentEl = document.getElementById('slider-content');
        this.onMouseMove = this.onMouseMove.bind(this);
        this.activeImg = el.getElementsByClassName(activeImgClass);
        this.activeText = el.getElementsByClassName(activeTextClass);
        this.images = el.getElementsByTagName('img');
        document.getElementById('sl-nav-dots').addEventListener('click', this.onDotClick.bind(this));
        document.getElementById('left').addEventListener('click', this.prev.bind(this));
        document.getElementById('right').addEventListener('click', this.next.bind(this));
        window.addEventListener('resize', this.onResize.bind(this));
        this.onResize();
        this.length = this.images.length;
        this.lastX = this.lastY = this.targetX = this.targetY = 0;
    }
    onResize() {
        const htmlStyles = getComputedStyle(document.documentElement);
        const mobileBreakpoint = htmlStyles.getPropertyValue('--mobile-bkp');
        const isMobile = this.isMobile = matchMedia(`only screen and (max-width: ${mobileBreakpoint})`).matches;
        this.halfWidth = this.contentE0.offsetWidth / 2;
        this.halfHeight = this.contentE0.offsetHeight / 2;
        this.zDistance = htmlStyles.getPropertyValue('--z-distance');
        if (!isMobile && !this.mouseWatched) {
            this.mouseWatched = true;
            this.el.addEventListener('mousemove', this.onMouseMove);
            this.el.style.setProperty(
                '--img-prev',
                `url(${this.images[+this.activeImg[0].dataset.id - 1].src})`
            );
            this.contentEl.style.setProperty('transform', `translateZ(${this.zDistance})`);
        } else if (isMobile && this.mouseWatched) {
            this.mouseWatched = false;
            this.el.removeEventListener('mousemove', this.onMouseMove);
            this.contentEl.style.setProperty('transform', 'none');
        }
    }
    getMouseCoefficients({ clientX, clientY } = {}) {
        const halfWidth = this.halfWidth;
        const halfHeight = this.halfHeight;
        const xCoeff = ((clientX || this.targetX) - halfWidth) / halfWidth;
        const yCoeff = (halfHeight - (clientY || this.targetY)) / halfHeight;
        return { xCoeff, yCoeff }
    }
    onMouseMove({ clientX, clientY }) {
        this.targetX = clientX - this.contentE0.getBoundingClientRect().left;
        this.targetY = clientY - this.contentE0.getBoundingClientRect().top;
        if (!this.animationRunning) {
            this.animationRunning = true;
            this.runAnimation();
        }
    }
    runAnimation() {
        if (this.animationStopped) {
            this.animationRunning = false;
            return;
        }
        const maxX = 10;
        const maxY = 10;
        const newPos = lerp({
            x: this.lastX,
            y: this.lastY
        }, {
            x: this.targetX,
            y: this.targetY
        });
        const { xCoeff, yCoeff } = this.getMouseCoefficients({
            clientX: newPos.x,
            clientY: newPos.y
        });
        this.lastX = newPos.x;
        this.lastY = newPos.y;
        this.positionImage({ xCoeff, yCoeff });
        this.contentEl.style.setProperty('transform', `
            translateZ(${this.zDistance})
            rotateX(${maxY * yCoeff}deg)
            rotateY(${maxX * xCoeff}deg)
        `);
        if (this.reachedFinalPoint) {
            this.animationRunning = false;
        } else {
            requestAnimationFrame(this.runAnimation.bind(this));
        }
    }
    get reachedFinalPoint() {
        const lastX = ~~this.lastX;
        const lastY = ~~this.lastY;
        const targetX = this.targetX;
        const targetY = this.targetY;
        return (lastX == targetX || lastX - 1 == targetX || lastX + 1 == targetX)
            && (lastY == targetY || lastY - 1 == targetY || lastY + 1 == targetY);
    }
    positionImage({ xCoeff, yCoeff }) {
        const maxImgOffset = 1;
        const currentImage = this.activeImg[0].children[0];
        currentImage.style.setProperty('transform', `
            translateX(${maxImgOffset * -xCoeff}em)
            translateY(${maxImgOffset * yCoeff}em)
        `);
    }
    onDotClick({ target }) {
        if (this.inTransit) return;
        const dot = target.closest('.sl-nav-dot');
        if (!dot) return;
        const nextId = dot.dataset.id;
        const currentId = this.activeImg[0].dataset.id;
        if (currentId == nextId) return;
        this.startTransition(nextId);
    }
    transitionItem(nextId) {
        function onImageTransitionEnd(e) {
            e.stopPropagation();
            nextImg.classList.remove(transitClass);
            self.inTransit = false;
            this.className = imgClass;
            this.removeEventListener('transitionend', onImageTransitionEnd);
        }
        const self = this;
        const el = this.el;
        const currentImg = this.activeImg[0];
        const currentId = currentImg.dataset.id;
        const imgClass = this.IMG_CLASS;
        const textClass = this.TEXT_CLASS;
        const activeImgClass = this.ACTIVE_IMG_CLASS;
        const activeTextClass = this.ACTIVE_TEXT_CLASS;
        const subActiveClass = `${imgClass}-subactive`;
        const transitClass = `${imgClass}-transit`;
        const nextImg = el.querySelector(`.${imgClass}[data-id='${nextId}']`);
        const nextText = el.querySelector(`.${textClass}[data-id='${nextId}']`);
        let outClass = '';
        let inClass = '';
        this.animationStopped = true;
        nextText.classList.add(activeTextClass);
        el.style.setProperty('--from-left', nextId);
        currentImg.classList.remove(activeImgClass);
        currentImg.classList.add(subActiveClass);
        if (currentId < nextId) {
            outClass = `${imgClass}-next`;
            inClass = `${imgClass}-prev`;
        } else {
            outClass = `${imgClass}-prev`;
            inClass = `${imgClass}-next`;
        }
        nextImg.classList.add(outClass);
        requestAnimationFrame(() => {
            nextImg.classList.add(transitClass, activeImgClass);
            nextImg.classList.remove(outClass);
            this.animationStopped = false;
            this.positionImage(this.getMouseCoefficients());
            currentImg.classList.add(transitClass, inClass);
            currentImg.addEventListener('transitionend', onImageTransitionEnd);
        });
        if (!this.isMobile)
            this.switchBackgroundImage(nextId);
    }
    startTransition(nextId) {
        function onTextTransitionEnd(e) {
            if (!e.pseudoElement) {
                e.stopPropagation();
                requestAnimationFrame(() => {
                    self.transitionItem(nextId);
                });
                this.removeEventListener('transitionend', onTextTransitionEnd);
            }
        }
        if (this.inTransit) return;
        const activeText = this.activeText[0];
        const backwardsClass = `${this.TEXT_CLASS}-backwards`;
        const self = this;
        this.inTransit = true;
        activeText.classList.add(backwardsClass);
        activeText.classList.remove(this.ACTIVE_TEXT_CLASS);
        activeText.addEventListener('transitionend', onTextTransitionEnd);
        requestAnimationFrame(() => {
            activeText.classList.remove(backwardsClass);
        });
    }
    next() {
        if (this.inTransit) return;
        let nextId = +this.activeImg[0].dataset.id + 1;
        if (nextId > this.length)
            nextId = 1;
        this.startTransition(nextId);
    }
    prev() {
        if (this.inTransit) return;
        let nextId = +this.activeImg[0].dataset.id - 1;
        if (nextId < 1)
            nextId = this.length;
        this.startTransition(nextId);
    }
    switchBackgroundImage(nextId) {
        function onBackgroundTransitionEnd(e) {
            if (e.target === this) {
                this.style.setProperty('--img-prev', imageUrl);
                this.classList.remove(bgClass);
                this.removeEventListener('transitionend', onBackgroundTransitionEnd);
            }
        }
        const bgClass = 'slider--bg-next';
        const el = this.el;
        const imageUrl = `url(${this.images[+nextId - 1].src})`;
        el.style.setProperty('--img-next', imageUrl);
        el.addEventListener('transitionend', onBackgroundTransitionEnd);
        el.classList.add(bgClass);
    }
}
const sliderEl = document.getElementById('slider');
const slider = new Slider(sliderEl);
let timer = 0;
function autoSlide() {
    requestAnimationFrame(() => {
        slider.next();
    });
    timer = setTimeout(autoSlide, 4000);
}
function stopAutoSlide() {
    clearTimeout(timer);
    this.removeEventListener('touchstart', stopAutoSlide);
    this.removeEventListener('mousemove', stopAutoSlide);
}
sliderEl.addEventListener('mousemove', stopAutoSlide);
sliderEl.addEventListener('touchstart', stopAutoSlide);
timer = setTimeout(autoSlide, 4000);


$('body').append('<div class="upbtn"></div>');
$(window).scroll(function () {
    if ($(this).scrollTop() > 100) {
        $('.upbtn').css({
            transform: 'scale(1)'
        });
    } else {
        $('.upbtn').css({
            transform: 'scale(0)'
        });
    }
});
$('.upbtn').on('click', function () {
    $('html, body').animate({
        scrollTop: 0
    }, 500);
    return false;
});

let cards = document.querySelectorAll(".cards");
for (let i = 0; i < cards.length; i++) {
    cards[i].onmousemove = e => {
        for (const card of document.getElementsByClassName("card")) {
            const rect = card.getBoundingClientRect(),
                x = e.clientX - rect.left,
                y = e.clientY - rect.top;
            card.style.setProperty("--mouse-x", `${x}px`);
            card.style.setProperty("--mouse-y", `${y}px`);
        };
    }
}

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

$('body').on('click', '.password-checkbox', function () {
    if ($(this).is(':checked')) {
        $('#password-input').attr('type', 'text');
    } else {
        $('#password-input').attr('type', 'password');
    }
});

function myFunction() {
    var x = document.getElementById("myInput");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

let activeIndex = 0
let limit = 0
let disabled = false
let $stage
let $controls
let canvas = false
const SPIN_FORWARD_CLASS = 'js-spin-fwd'
const SPIN_BACKWARD_CLASS = 'js-spin-bwd'
const DISABLE_TRANSITIONS_CLASS = 'js-transitions-disabled'
const SPIN_DUR = 1000
const appendControls = () => {
    $controls = $('.tabs3d-control').children()
    $controls.eq(activeIndex).addClass('active')
}
const setIndexes = () => {
    $('.spinner').children().each((i, el) => {
        if (i == 0) {
            $(el).addClass('js-active');
        }
        $(el).attr('data-index', i)
        $('.tabs3d-control').append('<a style="background-color: ' + $(el).attr('data-bg') + '" href="#tab" data-index="' + i + '">' + $(el).attr('data-menu') + '</a>');
        limit++
    })
}
const duplicateSpinner = () => {
    const $el = $('.spinner').parent()
    const html = $('.spinner').parent().html()
    $el.append(html)
    $('.spinner').last().addClass('spinner-right')
    $('.spinner-right').removeClass('spinner-left')
}
const paintFaces = () => {
    $('.spinner-face').each((i, el) => {
        $(el).children().css('background', $(el).attr('data-bg'))
    })
}
const prepareDom = () => {
    setIndexes()
    paintFaces()
    duplicateSpinner()
    appendControls()
}
const spin = (inc = 1) => {
    if (disabled) return
    if (!inc) return
    activeIndex += inc
    disabled = true
    if (activeIndex >= limit) {
        activeIndex = 0
    }
    if (activeIndex < 0) {
        activeIndex = (limit - 1)
    }
    const $activeEls = $('.spinner-face.js-active')
    const $nextEls = $('.spinner-face[data-index=' + activeIndex + ']')
    $nextEls.addClass('js-next')
    if (inc > 0) {
        $stage.addClass(SPIN_FORWARD_CLASS)
    } else {
        $stage.addClass(SPIN_BACKWARD_CLASS)
    }
    $controls.removeClass('active')
    $controls.eq(activeIndex).addClass('active')
    setTimeout(() => {
        spinCallback(inc)
    }, SPIN_DUR, inc)
}
const spinCallback = (inc) => {
    $('.js-active').removeClass('js-active')
    $('.js-next').removeClass('js-next').addClass('js-active')
    $stage
        .addClass(DISABLE_TRANSITIONS_CLASS)
        .removeClass(SPIN_FORWARD_CLASS)
        .removeClass(SPIN_BACKWARD_CLASS)
    $('.js-active').each((i, el) => {
        const $el = $(el)
        $el.prependTo($el.parent())
    })
    setTimeout(() => {
        $stage.removeClass(DISABLE_TRANSITIONS_CLASS)
        disabled = false
    }, 100)
}
const attachListeners = () => {
    document.onkeyup = (e) => {
        switch (e.keyCode) {
            case 38:
                spin(-1)
                break
            case 40:
                spin(1)
                break
        }
    }
    $controls.on('click', (e) => {
        e.preventDefault()
        if (disabled) return
        const $el = $(e.target)
        const toIndex = parseInt($el.attr('data-index'), 10)
        spin(toIndex - activeIndex)
    })
}
const assignEls = () => {
    $stage = $('.tabs3d-stage')
}
const init = () => {
    assignEls()
    prepareDom()
    attachListeners()
}
$(() => {
    init();
});

const counters = document.querySelectorAll('.value');
const speed = 200;

counters.forEach(counter => {
    const animate = () => {
        const value = +counter.getAttribute('akhi');
        const data = +counter.innerText;

        const time = value / speed;
        if (data < value) {
            counter.innerText = Math.ceil(data + time);
            setTimeout(animate, 1);
        } else {
            counter.innerText = value;
        }
    }
    animate();
});







var Boxlayout = (function () {
    var wrapper = document.body,
        sgroups = Array.from(document.querySelectorAll(".sgroup")),
        closeButtons = Array.from(document.querySelectorAll(".close-sgroup")),
        expandedClass = "is-expanded",
        hasExpandedClass = "has-expanded-item";
    return { init: init };
    function init() {
        _initEvents();
    }
    function _initEvents() {
        sgroups.forEach(function (element) {
            element.onclick = function () {
                _opensgroup(this);
            };
        });
        closeButtons.forEach(function (element) {
            element.onclick = function (element) {
                element.stopPropagation();
                _closesgroup(this.parentElement);
            };
        });
    }
    function _opensgroup(element) {
        if (!element.classList.contains(expandedClass)) {
            element.classList.add(expandedClass);
            wrapper.classList.add(hasExpandedClass);
        }
    }
    function _closesgroup(element) {
        if (element.classList.contains(expandedClass)) {
            element.classList.remove(expandedClass);
            wrapper.classList.remove(hasExpandedClass);
        }
    }
})();
Boxlayout.init();    

<<<<<<< HEAD

=======
>>>>>>> 35d5d4ab21124ebe257107379471e04322e10b47
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
<<<<<<< HEAD
tabToggle.first().trigger('click');  



=======
tabToggle.first().trigger('click');



function hide(obj) {
    var el = document.getElementById(obj);
    el.style.display = 'none';
}
>>>>>>> 35d5d4ab21124ebe257107379471e04322e10b47
