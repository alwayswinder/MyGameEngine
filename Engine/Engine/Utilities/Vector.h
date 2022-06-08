#pragma once
#include "CommonHeaders.h"


namespace primal::utl
{
	template<typename T, bool destruct=true>
	class vector
	{
	public:
		vector() = default;

		constexpr explicit vector(u64 count)
		{
			resize(count);
		}
		constexpr explicit vector(u64 count, const T& value)
		{
			resize(count, value);
		}
		constexpr vector(const vector& o)
		{
			*this = o;
		}
		constexpr vector(const vector&&o)
			:_capacity(o._capacity), _size{ o._size}, _data{o._data}
		{
			o.reset();
		}

		constexpr vector& operator=(const vector& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				clear();
				reserve(o._size);
				for (auto& item :o)
				{
					emplace_back(item);
				}
				assert(_size == o._size);
			}
			return *this;
		}

		constexpr vector& operator=(const vector&& o)
		{
			assert(this != std::addressof(o));
			if (this != std::addressof(o))
			{
				destroy();
				Move(o);
			}
			return *this;
		}
		~vector() { destroy(); }
	private:
		constexpr void move(vector& o)
		{
			_capacity = o._capacity;
			_size = o._size;
			_data = o._data;
			o.reset();
		}

		constexpr void reset()
		{
			_capacity = 0;
			_size = 0;
			_data = nullptr;
		}

		constexpr void destroy()
		{
			assert([&] {return _capacity ? _data != nullptr : _data == nullptr; }());
			clear();
			_capacity = 0;
			if (_data) free(_data);
			_data = nullptr;
		}

		u64 _capacity{ 0 };
		u64 _size{ 0 };
		T*	_data{ nullptr };
	};
}