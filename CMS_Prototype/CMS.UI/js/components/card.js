Vue.component('card-header', {
    props: ['block'],
    template:
        "<div class='card-header'>" +
            "<template v-if='block.Props.DisplayName'>" +
                "<h5>{{block.Props.DisplayName}}</h5>" +
            "</template>" +
            "<template v-else >" +
                "<div class='row'>" +
                    "<elm v-for='control in block.Controls' :block='control'></elm>" +
                "</div>" +
            "</template>" +
        "</div>"
});

Vue.component('card-body', {
    props: ['block'],
    template:
        "<div class='card-body'>" +
            "<div class='row'>" +
                "<elm v-for='block in block.Elements' :block='block'></elm>" +
            "</div>" +
        "</div>"
});

Vue.component('card-footer', {
    props: ['block'],
    template:
        "<div class='card-footer'>" +
            "<elm v-for='control in block.Controls' :block='control'></elm>" +
        "</div>"
});

Vue.component('view-card-complex', {
    props: ['block'],
    template:
        "<div class='card'>" +
            "<template v-for='block in block.Views'>" +

                "<card-header v-if=\"block.ViewType==='card-header'\" v-bind:block='block'></card-header>" +

                "<card-body v-if=\"block.ViewType==='card-body'\" v-bind:block='block'></card-body>" +

                "<card-footer v-if=\"block.ViewType==='card-footer'\" v-bind:block='block'></card-footer>" +

            "</template>" +
        "</div>"
});

Vue.component('view-card', {
    props: ['block'],
    template:
        "<div class='card'>" +
            "<div class='card-header'>" +
                "<template v-if='block.Props.DisplayName'>" +
                    "<h5>{{block.Props.DisplayName}}</h5>" +
                "</template>" +
            "</div>" +
            "<div class='card-body'>" +
                "<div class='row'>" +
                    "<elm v-for='block in block.Elements' v-if=\"!block.ControlType != 'button' \" :block='block'></elm>" +
                "</div>" +
            "</div>" +
            "<div class='card-footer'>" +
                "<elm v-for='control in block.Elements' v-if=\"!control.ControlType == 'button' \" :block='control'></elm>" +
            "</div>" +
        "</div>"
});
