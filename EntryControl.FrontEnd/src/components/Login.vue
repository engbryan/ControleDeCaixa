<template>
  <v-container class="d-flex justify-center align-center" fill-height>
    <v-card class="pa-4" min-width="400px">
      <v-card-title class="text-h5">Login</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="login">
          <v-text-field v-model="username"
                        label="Username"
                        required></v-text-field>
          <v-text-field v-model="password"
                        label="Password"
                        type="password"
                        required></v-text-field>
          <v-btn type="submit" color="primary" block>Login</v-btn>
          <v-alert v-if="error" type="error" outlined>{{ error }}</v-alert>
        </v-form>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script>
  import authHandler from '../services/authHandler';

  export default {
    name: 'Login',
    data() {
      return {
        username: '',
        password: '',
        error: null,
      };
    },
    methods: {
      async login() {
        try {
          await authHandler.login(this.username, this.password);
          this.$router.push('/reports');
        } catch (err) {
          this.error = 'Login failed. Please check your credentials and try again.';
        }
      },
    },
  };
</script>
