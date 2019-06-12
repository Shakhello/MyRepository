Vue.component('registryitem', {
    props: ['block'],
    template: "<li><div class='row'><control v-for='control in block.Controls'  :control='control'></control></div></li>"
});

Vue.component('registry', {
    props: ['block'],
    template:
        "<div class='card'>" +
            "<div class='card-header bg-success'><h5>{{block.Props.DisplayName}}</h5></div>" +
            "<div class='card-body'>" +
                "<view-table v-bind:block='block'></view-table>" +
            "</div>" +
            "<div class='card-footer'>" +
                "<pagination v-bind:block='block'></pagination>" +
            "</div>" +
        "</div>"
});