<template>
    <div class="Form">
        <PikLogo />
        <h2 class="Form-title">Личный кабинет</h2>
        <h3 v-show="welcomeMsg" class="Form-welcome">{{welcomeMsg}}</h3>
        <h3 v-show="errorMsg" class="Form-error">{{errorMsg}}</h3>
        <div class="AuthForm-form">
            <div class="AuthForm-formWrap">
                <div class="AuthForm-wrapper">
                    <ValidateInput id="login" placeholder="Логин" pattern="[A-Za-z0-9]" v-model="login.val" @validation="onValidation"></ValidateInput>
                    <ValidateInput id="lastName" placeholder="Фамилия" pattern="[А-Яа-я]" v-model="lastName.val" @validation="onValidation"></ValidateInput>
                    <ValidateInput id="firstName" placeholder="Имя" pattern="[А-Яа-я]" v-model="firstName.val" @validation="onValidation"></ValidateInput>
                </div>
                <CommitButton :disabled="!validateToCommit" @click.native="onSubmit">Отправить</CommitButton>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';

    import Provider from '../data/wcfProvider';

    import PikLogo from './PikLogo.vue';
    import ValidateInput from './ValidateInput.vue';
    import CommitButton from './CommitButton.vue';

    class ValidateValue {
        constructor(value: any) {
            this.val = value;
        }
        public val: any;
        public validate: boolean = false;
    }

    @Component({
        components: {
            PikLogo,
            ValidateInput,
            CommitButton
        }
    })
    export default class Login extends Vue {
        private login: ValidateValue = new ValidateValue("");
        private firstName: ValidateValue = new ValidateValue("");
        private lastName: ValidateValue = new ValidateValue("");

        private welcomeMsg: string = "";
        private errorMsg: string = "";

        get validateToCommit() {
            let result = this.login.validate && this.firstName.validate && this.lastName.validate
            return result;
        }

        onValidation(key: string, value: boolean) {
            this.welcomeMsg = "";
            this.errorMsg = "";
            this.$data[key].validate = value;
        }

        onSubmit() {
            let provider = new Provider();
            var data = {
                Login: this.login.val,
                FirstName: this.firstName.val,
                LastName: this.lastName.val
            };
            this.welcomeMsg = "";
            this.errorMsg = "";
            provider.saveAccount(data)
                .then(data => {
                    if (data.Status) {
                        this.welcomeMsg = "Добро пожаловать";
                    } else {
                        this.errorMsg = data.Msg;
                    }
                })
                .catch(error => console.error(error));
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .Form {
        z-index: 10;
        position: relative;
        padding: 60px 40px;
        max-width: 380px;
        width: 100%;
        display: -ms-flexbox;
        display: flex;
        text-align: center;
        -ms-flex-direction: column;
        flex-direction: column;
        -ms-flex-align: center;
        align-items: center;
        box-shadow: 0 20px 40px 1px rgba(0,0,0,.13);
        background: #fff;
        border: 1px solid #edeef0;
        border-radius: 4px;
        box-sizing: border-box;
        font-family: LabGrotesque;
    }

    .Form-title {
        margin-bottom: 40px;
        font-size: 30px;
        color: #484848;
        letter-spacing: 0;
        line-height: 35px;
        max-width: 297px;
        cursor: default;
    }

    .Form-welcome {
        margin-bottom: 20px;
        color: steelblue;
        max-width: 297px;
    }

    .Form-error {
        margin-bottom: 20px;
        color: red;
        max-width: 297px;
    }

    .AuthForm-form {
        z-index: 10;
        height: auto !important;
        display: -ms-flexbox;
        display: flex;
        -ms-flex-direction: column;
        flex-direction: column;
        -ms-flex-pack: justify;
        justify-content: space-between;
        max-width: 300px;
        width: 100%;
    }

    .AuthForm-wrapper {
        flex-grow: 1;
    }
</style>
