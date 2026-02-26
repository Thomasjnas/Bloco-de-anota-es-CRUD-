<template>
  <!-- FUNDO -->
  <div
    class="min-h-screen flex items-center justify-center p-6
           bg-gradient-to-br from-green-300 via-green-200 to-emerald-300"
  >
    <!-- CARD CENTRAL -->
    <div
      class="w-full max-w-sm bg-gray-900 rounded-2xl shadow-2xl p-8"
    >
      <!-- TÍTULO -->
      <h1 class="text-3xl font-bold text-white mb-6 text-center">
        Anotações
      </h1>

      <!-- INPUT NOVA TAREFA -->
      <input
        v-model="title"
        class="w-full bg-gray-100 text-gray-900 rounded-md px-3 py-2
               placeholder-gray-500 mb-3 outline-none
               focus:ring-2 focus:ring-blue-500"
        placeholder="Nova tarefa"
        @keydown.enter.prevent="createTask"
      />

      <!-- BOTÃO -->
      <button
        class="w-full bg-blue-600 hover:bg-blue-500 active:bg-blue-700
               text-white font-semibold py-2 rounded-md shadow mb-6 transition"
        @click="createTask"
      >
        Adicionar
      </button>

      <!-- LISTA -->
      <div class="space-y-2 max-h-64 overflow-auto pr-1">
        <p v-if="loading" class="text-gray-300 text-sm opacity-80">
          Carregando...
        </p>

        <div
          v-for="task in tasks"
          :key="task.id"
          class="flex items-center justify-between gap-2
                 bg-white/10 rounded-lg px-3 py-2"
        >
          <!-- MODO NORMAL -->
          <template v-if="editingId !== task.id">
            <label class="flex items-center gap-2 cursor-pointer flex-1">
              <input
                type="checkbox"
                :checked="task.done"
                @change="toggleDone(task.id)"
              />
              <span
                class="text-white text-sm"
                :class="task.done ? 'line-through opacity-60' : ''"
              >
                {{ task.title }}
              </span>
            </label>

            <div class="flex gap-2">
              <!-- EDITAR -->
              <button
                class="text-blue-300 hover:text-blue-500"
                title="Editar"
                @click="startEdit(task)"
              >
                ✏️
              </button>

              <!-- EXCLUIR -->
              <button
                class="text-red-300 hover:text-red-500"
                title="Excluir"
                @click="removeTask(task.id)"
              >
                ✖
              </button>
            </div>
          </template>

          <!-- MODO EDIÇÃO -->
          <template v-else>
            <input
              v-model="editingTitle"
              class="flex-1 rounded-md px-2 py-1 text-black"
            />

            <div class="flex gap-2">
              <!-- SALVAR -->
              <button
                class="text-green-400 hover:text-green-500"
                title="Salvar"
                @click="saveEdit(task.id)"
              >
                ✔
              </button>

              <!-- CANCELAR -->
              <button
                class="text-gray-300 hover:text-gray-400"
                title="Cancelar"
                @click="cancelEdit"
              >
                ✖
              </button>
            </div>
          </template>
        </div>

        <p
          v-if="!loading && tasks.length === 0"
          class="text-gray-300 text-sm opacity-80 text-center"
        >
          Nenhuma tarefa ainda.
        </p>
      </div>

      <!-- RODAPÉ -->
      <p class="text-xs text-gray-400 mt-5 text-center">
        API: {{ apiBase }}
      </p>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      apiBase: "http://localhost:5112/api/todo",

      tasks: [],
      title: "",
      loading: false,

      // controle de edição
      editingId: null,
      editingTitle: "",
    };
  },

  methods: {
    // READ
    async loadTasks() {
      this.loading = true;
      try {
        const res = await fetch(this.apiBase);
        this.tasks = await res.json();
      } finally {
        this.loading = false;
      }
    },

    // CREATE
    async createTask() {
      const text = this.title.trim();
      if (!text) return;

      await fetch(this.apiBase, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: text, done: false }),
      });

      this.title = "";
      await this.loadTasks();
    },

    // UPDATE (done)
    async toggleDone(id) {
      await fetch(`${this.apiBase}/${id}`, { method: "PUT" });
      await this.loadTasks();
    },

    // DELETE
    async removeTask(id) {
      await fetch(`${this.apiBase}/${id}`, { method: "DELETE" });
      await this.loadTasks();
    },

    // === EDIÇÃO ===
    startEdit(task) {
      this.editingId = task.id;
      this.editingTitle = task.title;
    },

    cancelEdit() {
      this.editingId = null;
      this.editingTitle = "";
    },

    async saveEdit(id) {
      const text = this.editingTitle.trim();
      if (!text) return;

      await fetch(`${this.apiBase}/${id}/title`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: text }),
      });

      this.cancelEdit();
      await this.loadTasks();
    },
  },

  mounted() {
    this.loadTasks();
  },
};
</script>
