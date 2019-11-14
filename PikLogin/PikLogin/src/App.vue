<template>
    <div id="app" class="AuthPage">
        <img class="AuthPage-img" src="https://0.pik.ru.cdn.pik-service.ru/specialoffers/0/139/214_0303_41db5c011229bdd82e2392ce9bfd8eb3.jpg">
        <Login msg="Hello world!" />
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Login from './components/Login.vue';

    @Component({
        components: {
            Login
        }
    })
    export default class App extends Vue { }
</script>

<style>
    html body {
        overflow-x: hidden;
        width: 100%;
        font-family: ProbaPro,Helvetica,Arial,sans-serif;
        font-size: 16px;
        text-rendering: optimizeLegibility;
        font-feature-settings: "kern";
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
    }

    .PageLayout-container {
        display: -ms-flexbox;
        display: flex;
        -ms-flex-pack: center;
        justify-content: center;
    }

    .AuthPage {
        height: 100vh;
        min-height: 568px;
        overflow: auto;
        width: 100vw;
        display: -ms-flexbox;
        display: flex;
        -ms-flex-pack: center;
        justify-content: center;
        -ms-flex-align: center;
        align-items: center;
        position: relative;
    }

    .AuthPage-img {
        position: absolute;
        top: 0;
        left: 0;
        object-fit: cover;
        width: 100%;
        height: 100%;
        z-index: 1;
    }
</style>
