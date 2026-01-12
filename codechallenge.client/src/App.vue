<template>
  <div class="search-container" v-if="headersReady">
    <h2>College Football Team Search</h2>

    <div class="form-row">
      <select v-model="selectedOption">
        <option value="0">All</option>
        <option v-for="item in searchOptions"
                :key="item.fileColumnHeaderID"
                :value="item.fileColumnHeaderID">
          {{ item.columnHeaderName }}
        </option>
      </select>

      <input type="text"
             v-model="searchText"
             placeholder="Search text" />

      <button @click="search">
        Search
      </button>
    </div>

    <table v-if="results.length > 0" class="results-table">
      <thead>
        <tr>
          <th v-for="item in searchOptions">{{ item.columnHeaderName }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in results" :key="row.id">
          <td>{{ row.rank }}</td>
          <td>{{ row.team }}</td>
          <td>{{ row.mascot }}</td>
          <td>{{ formatDate(row.lastWinDate) }}</td>
          <td>{{ row.percentage }}</td>
          <td>{{ row.wins }}</td>
          <td>{{ row.losses }}</td>
          <td>{{ row.ties }}</td>
          <td>{{ row.games }}</td>
        </tr>
      </tbody>
    </table>

    <p v-else-if="searched">No data found.</p>
  </div>
</template>

<script>
  import api from "@/services/api"
  export default {
    name: "SearchPage",
    data() {
      return {
        selectedOption: "0",
        searchText: "",
        searchOptions: [],
        results: [],
        searched: false,
        backendReady: false,
        headersReady: false,
      };
    },
    async mounted() {
      await this.waitForBackend();
      await this.loadHeaders();
    },
    methods: {
      async waitForBackend() {
        while (!this.backendReady) {
          try {
            const res = await api.get("/FileColumnHeaders/ready");
            this.backendReady = res.data.ready;
          } catch(err) {
            console.log("api error", err);
          }

          await new Promise(r => setTimeout(r, 3000));
        }
      },
      async loadHeaders() {
        await api
          .get("/FileColumnHeaders")
          .then((res) => {
            if (res.status == 200) {
              this.searchOptions = res.data;
              this.headersReady = true;
            }
          })
          .catch((err) => {
            console.log("api error", err);
          });
      },

      formatDate(dateString) {
        const date = new Date(dateString);

        const month = date.getMonth() + 1; 
        const day = date.getDate();
        const year = date.getFullYear().toString().slice(-2);

        return `${month}/${day}/${year}`;
      },

      async search() {
        if (!this.isValidSearch()) {
          return;
        }
        await api
          .get("/TeamStats", { params: { selectedOption: this.selectedOption, searchText: this.searchText.trim() } })
          .then((res) => {
            if (res.status == 200) {
              this.results = res.data;
            }
          })
          .catch((err) => {
            console.log("api error", err);
          });
        this.searched = true;
      },

      isValidSearch() {
        switch (this.selectedOption) {
          case 1:
          case 6:
          case 7:
          case 8:
          case 9: //int validation
            if (isNaN(parseInt(this.searchText.trim()))) {
              alert("Invalid number format.");
              return false;
            }
            return true;
          case 2:
          case 3: //empty text validation
            if (!this.searchText.trim()) {
              alert("Enter some text to search.");
              return false;
            }
            return true;
          case 4: //date validation
            if (!/^([1-9]|1[0-2])\/([1-9]|[12][0-9]|3[01])\/\d{2}$/.test(this.searchText)) {
              alert("Invalid date format");
              return false;
            }
            return true;
          case 5: //float validation
            if (isNaN(parseFloat(this.searchText.trim()))) {
              alert("Invalid number format.");
              return false;
            }
            return true;
          default:
            return true;
        }
      }
    }
  };
</script>

<style scoped>
  .search-container {
    max-width: 1000px;
    margin: 40px auto;
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 6px;
    font-family: Arial, sans-serif;
  }

  .form-row {
    display: flex;
    gap: 10px;
  }

  select,
  input {
    flex: 1;
    padding: 6px 8px;
    border: 1px solid #ccc;
    border-radius: 4px;
  }

  button {
    padding: 6px 12px;
    background-color: #007bff;
    border: none;
    color: white;
    border-radius: 4px;
    cursor: pointer;
  }

  button:hover {
    background-color: #0056b3;
  }

  .results-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
  }

  .results-table th,
  .results-table td {
    border: 1px solid #ddd;
    padding: 8px;
  }

  .results-table th {
    background-color: #f2f2f2;
    text-align: left;
  }
</style>
