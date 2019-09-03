function InitDragForm(source_list, target_list, bin, prefix) {

    new Sortable(source_list, {
        group: {
            name: prefix,
            pull: 'clone',
            put: false
        },
        animation: 150,
        sort: false
    });

    new Sortable(target_list, {
        group: {
            name: prefix
        },
        animation: 150
    });

    new Sortable(bin, {
        group: {
            name: prefix,
            pull: false
        },
        animation: 150
    });
    
    
    bin.addEventListener('drop', function() {
       EmptyBin(bin); 
    });
    
    target_list.addEventListener('drop', function() {
       EnableInput(target_list); 
    });
}

function EmptyBin(bin) {
    while(bin.firstChild)
        bin.removeChild(bin.firstChild);
}

function EnableInput(list) {
    var inputs = list.getElementsByTagName('input');
    for(i = 0; i < inputs.length; i++) {
        inputs[i].disabled = false;
    }
}

/**
 * Creates inputs with corresponding data in given form
 * @param container TargetList of some DragForm
 * @param form Form which is going to have new data added to it
 * @param prefix Prefix of name of added input elements
 */
function SubmitData(container, form, prefix) {
    var count = container.childElementCount;
    var count_input = document.createElement('input');
    count_input.name = prefix + '-count';
    count_input.value = count;
    count_input.style.display = 'none';
    form.appendChild(count_input);

    var childern = container.childNodes;
    var index = 0;
    childern.forEach(function(child) {
        if(child.tagName === 'DIV') {
            SubmitData_Element(child, form, prefix + '-' + index + '-');
            index++;
        }
    });

}

function SubmitData_Element(node, form, prefix) {
    var count = node.childElementCount;
    var count_input = document.createElement('input');
    count_input.name = prefix + '0';
    count_input.value = count;
    count_input.style.display = 'none';
    form.appendChild(count_input);

    var title = node.getElementsByClassName('title')[0].innerHTML;
    var title_input = document.createElement('input');
    title_input.name = prefix + '1';
    title_input.value = title;
    title_input.style.display = 'none';
    form.appendChild(title_input);

    var children = node.childNodes;
    var index = 2;
    children.forEach(function(child) {
        if(child.className === 'module-param') {
            var param_name = child.getElementsByTagName('span')[0].innerHTML;
            var param_name_input = document.createElement('input');
            param_name_input.name = prefix + index + '-name';
            param_name_input.value = param_name;
            param_name_input.style.display = 'none';
            form.appendChild(param_name_input);

            var param = child.getElementsByTagName('input')[0].value;
            var param_input = document.createElement('input');
            param_input.name = prefix + index;
            param_input.value = param;
            param_input.style.display = 'none';
            form.appendChild(param_input);
            index++;
        }
    });
}