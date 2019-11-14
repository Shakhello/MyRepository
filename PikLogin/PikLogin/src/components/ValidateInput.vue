<template>
    <label :for="id" class="Field">
        <input :id="id" :name="id" :placeholder="placeholder" class="Field-input" :value="value" @change="onChange">
        <span class="Field-icon">
            <slot></slot>
        </span>
        <span v-show="!validate" class="Field-invalid">Неверно введен '{{placeholder}}'</span>
    </label>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component
    export default class ValidateInput extends Vue {
        @Prop() id!: string;
        @Prop() value!: any;
        @Prop() placeholder!: string;
        @Prop() pattern!: string;

        validate: boolean = true;

        onChange($event: any) {
            this.$emit("input", $event.target.value);

            let regExp = new RegExp(this.pattern);
            this.validate = regExp.test($event.target.value);

            this.$emit("validation", this.id, this.validate);
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

    .Field {
        height: 60px;
        margin-bottom: 20px;
        margin-top: 0;
        display: block;
        position: relative;
        margin-top: 30px;
        width: 100%;
        text-align: left !important;
    }

    .Field-input {
        width: 100%;
        font-family: LabGrotesque;
        border: 1px solid #dbdbdb;
        height: 100%;
        border-radius: 4px;
        padding: 10px 50px 0 16px;
        box-sizing: border-box;
        font-size: 18px;
        color: #484848;
        font-weight: 300;
    }

    .Field-invalid {
        display: block;
        font-size: 12px;
        color: red;
        font-weight: 300;
    }

    .AuthForm-svg {
        position: absolute;
        right: 24px;
        top: 50%;
        width: 20px;
        -ms-transform: translateY(-50%);
        transform: translateY(-50%);
    }
</style>
