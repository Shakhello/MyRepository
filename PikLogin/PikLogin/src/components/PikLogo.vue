<template>
    <div class="Form-logo">
        <svg xmlns="http://www.w3.org/2000/svg" width="100%" height="100%" viewBox="0 0 40 40">
            <path fill="#0A055F" d="M20 0L0 10.2v4.3L20 4.4l20 10.1v-4.3zM3.4 18.9h4.8v9.8h3.4V15.8H0v12.9h3.4zM13.2 15.8v12.9h5.2l4.3-10.2v10.2h3.4V15.8H21l-4.4 10.3V15.8zM27.7 15.8v12.9h3.4v-4.1l1-1 3.9 5.1h4l-5.6-7.4 5.6-5.5h-4.4l-4.5 4.8v-4.8z"></path>
            <path fill="#FF4114" d="M34.3 33.4l-.8 2.2H35z"></path>
            <path fill="#FF4114" d="M0 40h40v-9.7H0V40zm33.4-7.6h1.8l2 5.5h-1.4l-.4-1.1h-2.3l-.4 1.1h-1.4l2.1-5.5zm-7.9 0h5v5.5h-1.3v-4.3h-2.4v4.3h-1.3v-5.5zm-6.1 0h5v5.5h-1.3v-4.3h-2.4v4.3h-1.3v-5.5zm-4.7 0l1.3 2.4 1.3-2.4h1.4l-3.1 5.5h-1.4l1.1-1.9-2.2-3.6h1.6zm-6.3 0h2.8c1.1 0 1.9.6 1.9 1.8 0 1.2-.9 1.8-1.9 1.8H9.7v1.9H8.4v-5.5zm-5 0h4.2v1.2H4.7v4.3H3.4v-5.5z"></path>
            <path fill="#FF4114" d="M11.9 34.2c0-.5-.3-.7-.8-.7H9.7v1.4h1.4c.4 0 .8-.2.8-.7z"></path>
        </svg>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';

    @Component
    export default class PikLogo extends Vue {

    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .Form-logo {
        width: 63px;
        height: 63px;
        margin-bottom: 40px;
    }
</style>
