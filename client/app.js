const API_BASE = "__API_BASE__";

const state = {
  restaurants: [],
  cart: JSON.parse(localStorage.getItem("fh_cart") || "[]"),
  cartRestaurant: JSON.parse(localStorage.getItem("fh_cart_rest") || "null"),
  token: localStorage.getItem("fh_token") || null,
  user: JSON.parse(localStorage.getItem("fh_user") || "null"),
  authMode: "login",
};

const $ = (id) => document.getElementById(id);
const money = (n) => "$" + Number(n).toFixed(2);

async function api(path, opts = {}) {
  const res = await fetch(API_BASE + path, {
    ...opts,
    headers: {
      "Content-Type": "application/json",
      ...(state.token ? { Authorization: "Bearer " + state.token } : {}),
      ...(opts.headers || {}),
    },
  });
  const data = await res.json().catch(() => ({}));
  if (!res.ok) throw new Error(data.error || "Something went wrong (" + res.status + ")");
  return data;
}

function toast(msg) {
  const t = $("toast");
  t.textContent = msg;
  t.classList.remove("hidden");
  setTimeout(() => t.classList.add("hidden"), 2600);
}

function saveCart() {
  localStorage.setItem("fh_cart", JSON.stringify(state.cart));
  localStorage.setItem("fh_cart_rest", JSON.stringify(state.cartRestaurant));
  renderCartCount();
}

function renderCartCount() {
  $("cart-count").textContent = state.cart.reduce((s, i) => s + i.qty, 0);
}

async function loadRestaurants(search) {
  const data = await api("/api/restaurants" + (search ? "?search=" + encodeURIComponent(search) : ""));
  state.restaurants = data;
  const grid = $("restaurant-grid");
  grid.innerHTML = "";
  if (data.length === 0) {
    grid.innerHTML = '<p style="color:var(--muted);grid-column:1/-1;text-align:center">No restaurants found.</p>';
    return;
  }
  for (const r of data) {
    const el = document.createElement("div");
    el.className = "card";
    el.innerHTML = `
      <div class="emoji">${r.emoji}</div>
      <div class="cuisine">${r.cuisine}</div>
      <h3>${r.name}</h3>
      <div class="desc">${r.description}</div>
      <div class="meta">
        <span class="rating">★ ${r.rating.toFixed(1)}</span>
        <span>🕒 ${r.deliveryMinutes} min</span>
        <span>🚚 ${money(r.deliveryFee)}</span>
      </div>`;
    el.onclick = () => openRestaurant(r);
    grid.appendChild(el);
  }
}

function openRestaurant(r) {
  const cats = [...new Set(r.menu.map((m) => m.category))];
  $("restaurant-detail").innerHTML = `
    <div style="font-size:2.6rem">${r.emoji}</div>
    <h3>${r.name}</h3>
    <p style="color:var(--muted);margin:6px 0">${r.description}</p>
    <div class="meta" style="display:flex;gap:12px;color:var(--muted);font-size:.85rem">
      <span class="rating" style="color:var(--accent2)">★ ${r.rating.toFixed(1)}</span>
      <span>🕒 ${r.deliveryMinutes} min</span><span>🚚 ${money(r.deliveryFee)}</span>
    </div>
    ${cats.map((c) => `
      <div class="menu-cat">${c}</div>
      ${r.menu.filter((m) => m.category === c).map((m) => `
        <div class="menu-item">
          <div class="info">
            <div class="name">${m.emoji} ${m.name}</div>
            <div class="desc">${m.description}</div>
          </div>
          <button class="add-btn" data-add="${m.id}" data-name="${m.name}" data-price="${m.price}">${money(m.price)} ＋</button>
        </div>`).join("")}`).join("")}`;
  $("restaurant-modal").classList.remove("hidden");
  $("restaurant-detail").querySelectorAll("[data-add]").forEach((b) => {
    b.onclick = () => {
      addToCart(r, { id: b.dataset.add, name: b.dataset.name, price: parseFloat(b.dataset.price) });
      b.textContent = "Added ✓";
      setTimeout(() => (b.innerHTML = money(parseFloat(b.dataset.price)) + " ＋"), 900);
    };
  });
}

function addToCart(restaurant, item) {
  if (state.cartRestaurant && state.cartRestaurant.id !== restaurant.id && state.cart.length > 0) {
    state.cart = [];
    toast("Cart cleared — new restaurant");
  }
  state.cartRestaurant = { id: restaurant.id, name: restaurant.name, fee: restaurant.deliveryFee };
  const line = state.cart.find((i) => i.id === item.id);
  if (line) line.qty++;
  else state.cart.push({ ...item, qty: 1 });
  saveCart();
  renderCart();
}

function changeQty(id, delta) {
  const line = state.cart.find((i) => i.id === id);
  if (!line) return;
  line.qty += delta;
  if (line.qty <= 0) state.cart = state.cart.filter((i) => i.id !== id);
  saveCart();
  renderCart();
}

function renderCart() {
  const box = $("cart-items");
  if (state.cart.length === 0) {
    box.innerHTML = '<p style="color:var(--muted);text-align:center;padding:30px 0">Your cart is empty.</p>';
  } else {
    box.innerHTML = `<p style="color:var(--accent2);font-size:.85rem;margin-bottom:6px">${state.cartRestaurant?.name || ""}</p>` +
      state.cart.map((i) => `
      <div class="cart-item">
        <div>${i.name}<div style="color:var(--muted);font-size:.8rem">${money(i.price)}</div></div>
        <div class="qty">
          <button data-dec="${i.id}">−</button><span>${i.qty}</span><button data-inc="${i.id}">＋</button>
        </div>
      </div>`).join("");
    box.querySelectorAll("[data-dec]").forEach((b) => (b.onclick = () => changeQty(b.dataset.dec, -1)));
    box.querySelectorAll("[data-inc]").forEach((b) => (b.onclick = () => changeQty(b.dataset.inc, 1)));
  }
  const sub = state.cart.reduce((s, i) => s + i.price * i.qty, 0);
  const fee = state.cart.length ? state.cartRestaurant?.fee || 0 : 0;
  $("cart-subtotal").textContent = money(sub);
  $("cart-fee").textContent = money(fee);
  $("cart-total").textContent = money(sub + fee);
}

async function checkout() {
  if (state.cart.length === 0) return toast("Your cart is empty");
  if (!state.token) return openAuth("login");
  const address = $("address").value.trim();
  if (!address) return toast("Please enter a delivery address");
  try {
    await api("/api/orders", {
      method: "POST",
      body: JSON.stringify({
        restaurantId: state.cartRestaurant.id,
        deliveryAddress: address,
        notes: $("notes").value.trim() || null,
        items: state.cart.map((i) => ({ menuItemId: i.id, quantity: i.qty })),
      }),
    });
    state.cart = [];
    saveCart();
    renderCart();
    closeDrawer();
    toast("Order placed! 🎉");
    showOrders();
  } catch (e) {
    if (e.message.includes("401")) openAuth("login");
    else toast(e.message);
  }
}

async function showOrders() {
  if (!state.token) return openAuth("login");
  $("view-home").classList.add("hidden");
  $("view-orders").classList.remove("hidden");
  const list = $("orders-list");
  list.innerHTML = "<p style='color:var(--muted)'>Loading…</p>";
  try {
    const orders = await api("/api/orders/mine");
    if (orders.length === 0) {
      list.innerHTML = "<p style='color:var(--muted)'>No orders yet. Go grab something tasty!</p>";
      return;
    }
    list.innerHTML = "";
    for (const o of orders) {
      const el = document.createElement("div");
      el.className = "order";
      el.innerHTML = `
        <div class="order-head">
          <strong>${o.restaurantName}</strong>
          <span class="badge ${o.status}">${o.status.replace("OnTheWay", "On the way")}</span>
        </div>
        <div class="items">${o.items.map((i) => i.quantity + "× " + i.name).join(", ")}</div>
        <div class="order-head">
          <span class="total">${money(o.total)}</span>
          <span style="color:var(--muted);font-size:.8rem">${new Date(o.createdAt).toLocaleString()}</span>
        </div>
        ${o.status !== "Delivered" && o.status !== "Cancelled" ? '<button class="advance">Advance status (demo)</button>' : ""}`;
      const adv = el.querySelector(".advance");
      if (adv) adv.onclick = async () => { await api(`/api/orders/${o.id}/advance`, { method: "POST" }); showOrders(); };
      list.appendChild(el);
    }
  } catch (e) {
    if (e.message.includes("401")) { openAuth("login"); return; }
    list.innerHTML = `<p style="color:var(--muted)">${e.message}</p>`;
  }
}

function openAuth(mode) {
  state.authMode = mode;
  $("auth-title").textContent = mode === "login" ? "Sign in" : "Create account";
  $("auth-submit").textContent = mode === "login" ? "Sign in" : "Register";
  $("auth-name").classList.toggle("hidden", mode === "login");
  $("auth-toggle").textContent = mode === "login" ? "Need an account? Register" : "Have an account? Sign in";
  $("auth-error").classList.add("hidden");
  $("auth-modal").classList.remove("hidden");
}

async function submitAuth() {
  const body = {
    name: $("auth-name").value.trim(),
    email: $("auth-email").value.trim(),
    password: $("auth-password").value,
  };
  try {
    const data = await api("/api/auth/" + state.authMode, { method: "POST", body: JSON.stringify(body) });
    state.token = data.token;
    state.user = { name: data.name, email: data.email };
    localStorage.setItem("fh_token", data.token);
    localStorage.setItem("fh_user", JSON.stringify(state.user));
    $("auth-modal").classList.add("hidden");
    updateAuthButton();
    toast("Welcome, " + data.name.split(" ")[0] + "!");
  } catch (e) {
    const err = $("auth-error");
    err.textContent = e.message;
    err.classList.remove("hidden");
  }
}

function updateAuthButton() {
  $("nav-auth").textContent = state.user ? state.user.name.split(" ")[0] + " ⏻" : "Sign in";
}

function closeDrawer() {
  $("cart-drawer").classList.add("hidden");
  $("drawer-backdrop").classList.add("hidden");
}

$("search").addEventListener("input", (e) => loadRestaurants(e.target.value));
$("nav-home").onclick = () => { $("view-orders").classList.add("hidden"); $("view-home").classList.remove("hidden"); };
$("nav-orders").onclick = showOrders;
$("nav-auth").onclick = () => {
  if (state.user) {
    state.token = null; state.user = null;
    localStorage.removeItem("fh_token"); localStorage.removeItem("fh_user");
    updateAuthButton(); toast("Signed out");
  } else openAuth("login");
};
$("auth-toggle").onclick = () => openAuth(state.authMode === "login" ? "register" : "login");
$("auth-submit").onclick = submitAuth;
$("cart-open").onclick = () => { renderCart(); $("cart-drawer").classList.remove("hidden"); $("drawer-backdrop").classList.remove("hidden"); };
$("cart-close").onclick = closeDrawer;
$("drawer-backdrop").onclick = closeDrawer;
$("checkout-btn").onclick = checkout;
document.querySelectorAll("[data-close]").forEach((b) => (b.onclick = () => $(b.dataset.close).classList.add("hidden")));
$("restaurant-modal").onclick = (e) => { if (e.target.id === "restaurant-modal") e.target.classList.add("hidden"); };
$("auth-modal").onclick = (e) => { if (e.target.id === "auth-modal") e.target.classList.add("hidden"); };

renderCartCount();
updateAuthButton();
loadRestaurants();
