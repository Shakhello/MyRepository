<template>
    <button class="Button" :disabled="disabled">
        <span class="Button-content">
            <slot></slot>
        </span>
    </button>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component
    export default class Button extends Vue {
        @Prop() private disabled!: string;
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .Button {
        height: 60px;
        text-align: center;
        width: 100%;
        max-width: 300px;
        background: #5a7788;
        border-radius: 4px;
        font-size: 18px;
        color: #fff;
        font-family: LabGrotesque;
        cursor: pointer;
        transition: background-color .4s ease;
    }

    .Button[disabled] {
        background: #f4f4f4;
        color: #dbdbdb;
        pointer-events: none;
    }
</style>
