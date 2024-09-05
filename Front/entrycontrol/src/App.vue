<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>EntryControl</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn v-if="isAuthenticated" @click="logout" color="secondary">Logout</v-btn>
    </v-app-bar>
    <v-main>
      <router-view></router-view>
    </v-main>
  </v-app>
</template>

<script>
  import authHandler from './services/authHandler';

  export default {
    name: 'App',
    computed: {
      isAuthenticated() {
        return authHandler.isAuthenticated();
      },
    },
    methods: {
      logout() {
        authHandler.logout();
        this.$router.push('/login');
      },
    },
  };
</script>
