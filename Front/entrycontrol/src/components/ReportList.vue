<template>
  <v-container>
    <v-card class="pa-4" outlined>
      <v-card-title class="headline">Daily Reports</v-card-title>
      <v-card-text>
        <v-row>
          <v-col v-for="report in reports"
                 :key="report.id"
                 cols="12"
                 md="6"
                 lg="4">
            <v-card outlined>
              <v-card-title>
                <v-icon left>mdi-calendar</v-icon>
                {{ new Date(report.ReportDate).toLocaleDateString() }}
              </v-card-title>
              <v-card-subtitle class="text-h6 grey--text">
                Credits: {{ report.TotalCredits }} | Debits: {{ report.TotalDebits }}
              </v-card-subtitle>
              <v-card-text>
                <v-chip :color="report.balance >= 0 ? 'green lighten-1' : 'red lighten-1'"
                        text-color="white"
                        class="ma-2">
                  Balance: {{ report.Balance }}
                </v-chip>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script>
  import reportService from '../services/reportService';

  export default {
    name: 'ReportList',
    data() {
      return {
        reports:  [],
      };
    },
    async created() {
      this.reports = await reportService.getReports();
    }
  };
</script>

<style scoped>
  .v-card-title {
    font-weight: 500;
  }

  .v-card-subtitle {
    font-size: 1rem;
  }
</style>
