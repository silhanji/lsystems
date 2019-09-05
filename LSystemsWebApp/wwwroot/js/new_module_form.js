var param_container = document.getElementById("param-container");

function remove_parent(node) {
    node.parentNode.remove(node.parentNode);
}


document.getElementById("add-param-button")
    .addEventListener("click", function() {
        var container = document.createElement('span');
        var input = document.createElement('input');
        input.type = 'text';
        input.className = 'form-field';
        input.name = 'param-name';
        var button = document.createElement('button');
        button.addEventListener('click', function() {remove_parent(button)});
        button.className = 'flat-button';
        var image = document.createElement('img');
        image.src = '/svg/clear.svg';
        button.appendChild(image);
        container.appendChild(input);
        container.appendChild(button);

        var br= document.createElement('br');
        var desc = document.createElement('span');
        desc.className = 'form-desc';

        container.appendChild(br);
        container.appendChild(desc);

        param_container.appendChild(container);
    });

var representation_select = document.getElementById("representation-select");
var distance = document.getElementById("line-length");
var line_color = document.getElementById("line-color");
var polygon_select = document.getElementById("polygon-select");
var polygon_fill = document.getElementById("polygon-fill");
var polygon_stroke = document.getElementById("polygon-stroke");
var rotation_angle = document.getElementById("rotation-angle");
var polygon_select_input = document.getElementById("polygon-select-select");

function show(node) {
    node.style.display = 'unset';
}

function hide(node) {
    node.style.display = 'none';
}

representation_select.addEventListener("change", function() {
    switch (representation_select.value) {
        case 'none':
            hide(distance);
            hide(line_color);
            hide(polygon_select);
            hide(polygon_fill);
            hide(polygon_stroke);
            hide(rotation_angle);
            break;
        case 'line':
            show(distance);
            show(line_color);
            hide(polygon_select);
            hide(polygon_fill);
            hide(polygon_stroke);
            hide(rotation_angle);
            break;
        case 'polygon':
            hide(distance);
            hide(line_color);
            show(polygon_select);
            hide(polygon_fill);
            hide(polygon_stroke);
            hide(rotation_angle);
            polygon_select_input.dispatchEvent(new Event('change'));
            break;
        case 'move':
            show(distance);
            hide(line_color);
            hide(polygon_select);
            hide(polygon_fill);
            hide(polygon_stroke);
            hide(rotation_angle);
            break;
        case 'rotation':
            hide(distance);
            hide(line_color);
            hide(polygon_select);
            hide(polygon_fill);
            hide(polygon_stroke);
            show(rotation_angle);
            break;
    }
});

polygon_select_input.addEventListener('change', function() {
    if(polygon_select_input.value == 'create-polygon') {
        show(polygon_fill);
        show(polygon_stroke);
    } else {
        hide(polygon_fill);
        hide(polygon_stroke);
    }
});

representation_select.dispatchEvent(new Event('change'));